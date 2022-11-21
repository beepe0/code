package chat;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.Socket;
import java.util.ArrayList;

//temp
import java.time.format.DateTimeFormatter;  
import java.time.LocalDateTime;   
//temp

public class ClientController implements Runnable{
    public static ArrayList<ClientController> globalClients = new ArrayList<>();
    public static ArrayList<UserDialog> dialogs = new ArrayList<>();
    private ArrayList<ClientController> currClients = globalClients;
    private UserDialog currDialog = null;
    private Socket cl = null;
    private ObjectInputStream input = null;
    private ObjectOutputStream output = null;

    public String nickname = null;
    private static short sCountOfClients = -1;
    public short sIndex = -1;

    public ClientController(Socket cl){
        try{
            this.cl = cl;
            this.output = new ObjectOutputStream(this.cl.getOutputStream());
            this.input = new ObjectInputStream(this.cl.getInputStream());
            
            Packet pack = (Packet)input.readObject();
            
            if(pack.typePacket == TypePacket.TOP_NICKNAME)
                this.nickname = (String) pack.bag;
            else this.nickname = "none";
            sCountOfClients++;
            sIndex = sCountOfClients;
            globalClients.add(this);
        }
        catch (IOException e){
            e.printStackTrace();
            closeClient();
        }
        catch (ClassNotFoundException e){
            e.printStackTrace();
            closeClient();
        }
    }
    
    private void closeClient(){
        try{
            removeFromServer(this);
            if(cl != null) cl.close();
            if(input != null) input.close();
            if(output != null) output.close();
        }
        catch (IOException e){
            e.printStackTrace();
        }

    }
               
    private void sendMessage(ArrayList<ClientController> cls, Packet pack){
        for(ClientController c: cls){
            try{
                if(c.sIndex != this.sIndex){
                    c.output.writeObject(pack);
                    c.output.flush();
                }
            }
            catch(IOException e){
                e.printStackTrace();
            }
        }
    }
   
     private void sendMessageOnlyOne(ClientController cl, Packet pack){
            try{
                cl.output.writeObject(pack);
                cl.output.flush();     
            }
            catch(IOException e){
                e.printStackTrace();
            }
    }
     
    private void removeFromServer(ClientController cc){
        sendMessage(currClients, new Packet(TypePacket.TOP_MESSAGE, "-Log: " + cc.nickname + " was disconnected!"));
        if(currDialog != null){
            currDialog.numbers.remove(this);
            currDialog = null;
        }
    }
    
    @Override
    public void run(){
        while (cl.isConnected()){
            try{
                Packet pack = (Packet)input.readObject();
                switch(pack.typePacket){
                    case TypePacket.TOP_MESSAGE:
                        sendMessage(currClients, new Packet(TypePacket.TOP_MESSAGE, ((String)pack.bag)));
                        System.out.println(Color.ANSI_YELLOW + "-Log: " + Color.ANSI_RESET + nickname + " send a message: " + ((String) pack.bag).length());
                    break;
                    case TypePacket.TOP_CREATEDIALOG:
                        UserDialog dlg = (UserDialog)pack.bag;
                        DateTimeFormatter dtf = DateTimeFormatter.ofPattern("yyyy/MM/dd HH:mm:ss");  
                        LocalDateTime now = LocalDateTime.now(); 
                        dlg.creationTime = dtf.format(now);
                        dialogs.add(dlg);
                        System.out.println(Color.ANSI_YELLOW + "-Log: " + Color.ANSI_RESET + nickname + " created a dialog: " + "Title: "+ Color.ANSI_CYAN + dlg.title + Color.ANSI_RESET + ", Number of users: " + Color.ANSI_CYAN + dlg.numberOfUser + Color.ANSI_RESET);
                    break;
                    case TypePacket.TOP_GETLISTOFDIALOGS:
                        String[][] dlgs = new String[dialogs.size()][3];
                        dlgs[0][2] = Integer.toString(dialogs.size());
                        int id = 0;
                        for(UserDialog ud : dialogs){
                            dlgs[id][0] = ud.title + "\t" + "------" + "\t" + "------" + "\t" + ud.creationTime + "\n";
                            dlgs[id][1] = ud.title;
                            id++;
                        }
                        sendMessageOnlyOne(this, new Packet(TypePacket.TOP_GETLISTOFDIALOGS, dlgs));
                    break;
                    case TypePacket.TOP_JOIN2DIALOG:
                        for(UserDialog ud: dialogs){
                            if(ud.title.equals((String)pack.bag)){
                                currDialog = ud;
                                currClients = ud.numbers;
                                ud.numbers.add(this);
                                globalClients.remove(this); 
                                sendMessage(currClients, new Packet(TypePacket.TOP_MESSAGE, ("-Log: " + this.nickname + " connected!")));
                                System.out.println(Color.ANSI_YELLOW + "-Log: " + Color.ANSI_RESET + nickname + " connected to " + Color.ANSI_CYAN + ud.title + Color.ANSI_RESET);
                            }
                        }
                    break;
                }
            }
            catch (IOException e){
                closeClient();
                System.out.println(Color.ANSI_CYAN + "-Log: " + Color.ANSI_RESET + nickname + " was disconnected!");
                break;
            }
            catch (ClassNotFoundException e) {
                closeClient();
                System.out.println(Color.ANSI_CYAN + "-Log: " + Color.ANSI_RESET + nickname + " was disconnected!");
                break;
            }
        }
    }
}