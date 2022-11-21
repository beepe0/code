package chat;

import java.io.IOException;
import java.net.Socket;
import java.nio.charset.Charset;

import com.googlecode.lanterna.TerminalSize;
import com.googlecode.lanterna.gui2.BasicWindow;
import com.googlecode.lanterna.gui2.Button;
import com.googlecode.lanterna.gui2.GridLayout;
import com.googlecode.lanterna.gui2.Label;
import com.googlecode.lanterna.gui2.MultiWindowTextGUI;
import com.googlecode.lanterna.gui2.Window;
import com.googlecode.lanterna.gui2.WindowBasedTextGUI;
import com.googlecode.lanterna.gui2.Button.Listener;
import com.googlecode.lanterna.screen.Screen;
import com.googlecode.lanterna.screen.TerminalScreen;
import com.googlecode.lanterna.terminal.DefaultTerminalFactory;
import com.googlecode.lanterna.terminal.Terminal;

public class Client{
    public static UserClient client = null;

    private static Terminal terminal = null;
    public static Terminal getTerminal(){
        return terminal;
    }
    private static Screen mainScreen = null;
    public static Screen getScreen(){
        return mainScreen;
    }
    private static Window win = null;
    public static Window getWindow(){
        return win;
    }
    private static Window win1 = null;
    public static Window getWindow1(){
        return win1;
    }
    private static WindowBasedTextGUI wbtGUI = null;
    public static WindowBasedTextGUI getWBasedTextGUI(){
        return wbtGUI;
    }

    public static void main(String[] a) throws IOException{
        try{  
            terminal = new DefaultTerminalFactory(System.out, System.in, Charset.forName(System.getProperty("file.encoding"))).createTerminal();
            mainScreen = new TerminalScreen(terminal);
            win = new BasicWindow("Chat");
            wbtGUI = new MultiWindowTextGUI(mainScreen);
        
            mainScreen.startScreen();
            UIController.initComponnent();
            UIController.listenersWindowns();
            
            win.setComponent(UIController.contentPanel);
            wbtGUI.addWindowAndWait(win);
            mainScreen.stopScreen();
            
        }catch(Exception e){
            e.printStackTrace();
        }

        finally{
            try {
                if(mainScreen != null){
                    mainScreen.stopScreen();
                }
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }
    public static boolean Connect(String name){
        try {
            client = new UserClient(new Socket("192.168.2.109", (short)4444), name.trim());
        } catch (IOException e) {
            e.printStackTrace();
        }
        if(client != null) return true;
        else return false;
    }
    public static void join(String[][] listOfDialogs){
        UIController.contentPanel2.addComponent(new Label("-Title-  \t-Slots-\t-Password-\t-Time-").setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.CENTER, GridLayout.Alignment.CENTER, true, true, 1,1)));
        UIController.contentPanel2.addComponent(new Label("# of dialogs: " + listOfDialogs.length).setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.CENTER, GridLayout.Alignment.CENTER, true, true, 1,1)));
        
        for(int i = 0; i < Integer.parseInt(listOfDialogs[0][2]); i++){
            String s = listOfDialogs[i][0];
            Label t = new Label(s);
            Button b = new Button("Join");
            int id = i;
            t.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.FILL, GridLayout.Alignment.CENTER, true, true, 1,1));
            b.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.FILL, GridLayout.Alignment.CENTER, true, true, 1,1));
            b.addListener(new Listener() {
                @Override
                public void onTriggered(Button button) {
                    win.setComponent(UIController.contentPanel4);
                    client.sendMessage(new Packet(TypePacket.TOP_JOIN2DIALOG, listOfDialogs[id][1]));
                }
            });
            UIController.contentPanel2.addComponent(t);
            UIController.contentPanel2.addComponent(b);
        }
        win.setComponent(UIController.contentPanel2);
    }
    
    public static void create(UserDialog udlg){
        client.sendMessage(new Packet(TypePacket.TOP_CREATEDIALOG, udlg));
        win.setComponent(UIController.contentPanel1);
    }

}

