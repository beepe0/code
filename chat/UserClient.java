package chat;

import java.io.IOException;
import java.net.Socket;

import java.io.ObjectOutputStream;
import java.io.ObjectInputStream;

public class UserClient{
    public Socket client = null;
    public ObjectInputStream input = null;
    public ObjectOutputStream output = null;

    public String nickName = null;
    
    public UserClient(Socket client, String nickName){
        try{
            this.client = client;
            this.nickName = nickName;

            this.output = new ObjectOutputStream(this.client.getOutputStream());
            this.input = new ObjectInputStream(this.client.getInputStream());
            sendMessage(new Packet(TypePacket.TOP_NICKNAME, nickName));
        }
        catch (IOException e){
            e.printStackTrace();
        }
    }
    
    public void closeClient(){
        try{
            Client.getScreen().stopScreen();
            client.close();
            input.close();
            output.close();
        }
        catch (IOException e){
            e.printStackTrace();
        }
    }

    public void sendMessage(Packet pack){
        try{
            output.writeObject(pack);
            output.flush();
        }
        catch (IOException e){
            e.printStackTrace();
            closeClient();
        }
    }
    public void threadReceivingMessage(){
        new Thread(new Runnable(){
            @Override
            public void run(){
                while (client.isConnected()){
                    try{
                        Packet pack = (Packet)input.readObject();
                        switch(pack.typePacket){
                            case TypePacket.TOP_MESSAGE:
                                UIController.showMess((String)pack.bag);
                            break;
                            case TypePacket.TOP_GETLISTOFDIALOGS:
                                String[][] dlgs = (String[][])pack.bag;
                                if(dlgs != null)Client.join(dlgs);
                            break;
                        }
                    }
                    catch (Exception e){
                        e.printStackTrace();
                        closeClient();
                        break;
                    }
                }
            }
        }).start();
    }
}