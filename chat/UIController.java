package chat;

import java.io.IOException;
import java.util.ArrayList;
import java.util.concurrent.atomic.AtomicBoolean;

import com.googlecode.lanterna.SGR;
import com.googlecode.lanterna.TerminalPosition;
import com.googlecode.lanterna.TerminalSize;
import com.googlecode.lanterna.gui2.Button;
import com.googlecode.lanterna.gui2.Direction;
import com.googlecode.lanterna.gui2.GridLayout;
import com.googlecode.lanterna.gui2.Label;
import com.googlecode.lanterna.gui2.Panel;
import com.googlecode.lanterna.gui2.Separator;
import com.googlecode.lanterna.gui2.TextBox;
import com.googlecode.lanterna.gui2.Window;
import com.googlecode.lanterna.gui2.WindowBasedTextGUI;
import com.googlecode.lanterna.gui2.WindowListener;
import com.googlecode.lanterna.gui2.Button.Listener;
import com.googlecode.lanterna.gui2.dialogs.MessageDialog;
import com.googlecode.lanterna.gui2.dialogs.MessageDialogBuilder;
import com.googlecode.lanterna.gui2.dialogs.MessageDialogButton;
import com.googlecode.lanterna.input.KeyStroke;
import com.googlecode.lanterna.input.KeyType;

class UIController{
    public static Panel contentPanel = new Panel().setLayoutManager(new GridLayout(3));
    public static Label labelName = new Label("Nickname: ");
    public static Separator separator1 = new Separator(Direction.VERTICAL);
    public static TextBox inputName = new TextBox();
    public static Button submitButton = new Button("Submit");

    public static Panel contentPanel1 = new Panel().setLayoutManager(new GridLayout(1));
    public static Label labelJoin = new Label("-Join to room-");
    public static Button buttonJoin = new Button("Join");
    public static Separator separator2 = new Separator(Direction.HORIZONTAL);
    public static Label labelCreate = new Label("-Create a room-");
    public static Button buttonCreate = new Button("Create");
    public static Separator separator3 = new Separator(Direction.HORIZONTAL);
    public static Button buttonBack = new Button("Back");

    public static Panel contentPanel2 = new Panel().setLayoutManager(new GridLayout(2));

    public static Panel contentPanel3 = new Panel().setLayoutManager(new GridLayout(1));
    public static Label labelTitle = new Label("-Title-");
    public static TextBox inputTitle = new TextBox();
    public static Separator separator4 = new Separator(Direction.HORIZONTAL);
    public static Button buttonSubmit = new Button("Submit");

    public static Panel contentPanel4 = new Panel().setLayoutManager(new GridLayout(1));
    public static Panel contentPanel4_1 = new Panel().setLayoutManager(new GridLayout(1));
    public static Separator separator5 = new Separator(Direction.HORIZONTAL);
    public static TextBox inputMess = new TextBox();
    public static Button buttonSend = new Button("Send");

    public static ArrayList<Label> arrayList = new ArrayList<>();
    public static int maxY, scroll;
    private static int currPos;


    public static void initComponnent(){
        labelName.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.CENTER, GridLayout.Alignment.CENTER, true, false, 1, 1));
        separator1.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.BEGINNING, GridLayout.Alignment.FILL, true, false, 1, 1));
        inputName.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.FILL, GridLayout.Alignment.CENTER, true, false, 1, 1));
        submitButton.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.CENTER, GridLayout.Alignment.CENTER, true, false, 3, 1));

        labelJoin.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.FILL, GridLayout.Alignment.CENTER, true, false, 1, 1));
        labelJoin.addStyle(SGR.BOLD);
        buttonJoin.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.FILL, GridLayout.Alignment.CENTER, false, false, 1, 1));
        separator2.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.FILL, GridLayout.Alignment.CENTER, true, false, 1, 1));
        labelCreate.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.FILL, GridLayout.Alignment.CENTER, true, false, 1, 1));
        labelCreate.addStyle(SGR.BOLD);
        buttonCreate.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.FILL, GridLayout.Alignment.CENTER, true, false, 1, 1));
        separator3.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.FILL, GridLayout.Alignment.CENTER, true, false, 1, 1));
        buttonBack.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.CENTER, GridLayout.Alignment.CENTER, true, false, 1, 1));
        
        labelTitle.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.CENTER, GridLayout.Alignment.CENTER, true, false, 1, 1));
        inputTitle.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.CENTER, GridLayout.Alignment.CENTER, true, false, 1, 1));
        separator4.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.FILL, GridLayout.Alignment.CENTER, true, false, 1, 1));
        buttonSubmit.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.CENTER, GridLayout.Alignment.CENTER, true, false, 1, 1));
        
        contentPanel4_1.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.CENTER, GridLayout.Alignment.CENTER, true, false, 1, 1));
        separator5.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.FILL, GridLayout.Alignment.CENTER, true, false, 1, 1));
        inputMess.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.FILL, GridLayout.Alignment.CENTER, true, false, 1, 1));
        buttonSend.setLayoutData(GridLayout.createLayoutData(GridLayout.Alignment.CENTER, GridLayout.Alignment.CENTER, true, false, 1, 1));

        buttonJoin.addListener(new Listener() {
            @Override
            public void onTriggered(Button button) {
                Client.client.threadReceivingMessage();
                Client.client.sendMessage(new Packet(TypePacket.TOP_GETLISTOFDIALOGS, null));
            }  
        });
        buttonCreate.addListener(new Listener() {
            @Override
            public void onTriggered(Button button) {
                Client.getWindow().setComponent(contentPanel3);
            }
        });
        buttonSubmit.addListener(new Listener() {
            @Override
            public void onTriggered(Button button) {
                Client.create(new UserDialog(inputTitle.getText(), (byte)2, null)); 
            }
            
        });
        buttonSend.addListener(new Listener() {
            @Override
            public void onTriggered(Button button) {
                if(inputMess.getText().trim().isEmpty()) return;
                String text = Client.client.nickName + " -> " + inputMess.getText();
                Client.client.sendMessage(new Packet(TypePacket.TOP_MESSAGE, text));
                showMess(text);
                inputMess.setText("");
            }
            
        });
        buttonBack.addListener(new Listener() {
            @Override
            public void onTriggered(Button button) {
                if(MessageDialog.showMessageDialog(Client.getWBasedTextGUI(), "Warning", "Info: Do you really want to exit?", MessageDialogButton.No, MessageDialogButton.Yes) == MessageDialogButton.Yes){
                    try {
                        Client.getScreen().stopScreen();
                    } catch (IOException e) {
                        e.printStackTrace();
                    }
                }
            }  
        });
        submitButton.addListener(new Listener() {
            @Override
            public void onTriggered(Button button) {
                if(Client.Connect(inputName.getText())) {
                    MessageDialog.showMessageDialog(Client.getWBasedTextGUI(), "Success", "Info: Connection is success!", MessageDialogButton.OK);
                    Client.getWindow().setComponent(contentPanel1);
                }
                else{ 
                    MessageDialog.showMessageDialog(Client.getWBasedTextGUI(), "Error", "Info: Connection isn`t success!", MessageDialogButton.OK);
                    try {
                        Client.getScreen().stopScreen();
                    } catch (IOException e) {
                        e.printStackTrace();
                    }
                }
            }
        });
        contentPanel.addComponent(labelName);
        contentPanel.addComponent(separator1);
        contentPanel.addComponent(inputName);
        contentPanel.addComponent(submitButton);

        contentPanel1.addComponent(labelJoin);
        contentPanel1.addComponent(buttonJoin);
        contentPanel1.addComponent(separator2);
        contentPanel1.addComponent(labelCreate);
        contentPanel1.addComponent(buttonCreate);
        contentPanel1.addComponent(separator3);
        contentPanel1.addComponent(buttonBack);

        contentPanel3.addComponent(labelTitle);
        contentPanel3.addComponent(inputTitle);
        contentPanel3.addComponent(separator4);
        contentPanel3.addComponent(buttonSubmit);
        
        contentPanel4.addComponent(contentPanel4_1);
        contentPanel4.addComponent(separator5);
        contentPanel4.addComponent(inputMess);
        contentPanel4.addComponent(buttonSend);

    }
    public static void listenersWindowns(){
        Client.getWindow().addWindowListener(new WindowListener() {
            @Override
            public void onInput(Window basePane, KeyStroke keyStroke, AtomicBoolean deliverEvent) {
                if(keyStroke.getKeyType() == KeyType.ArrowRight){
                    scroll--;
                    scrollList(contentPanel4_1);
                }else if(keyStroke.getKeyType() == KeyType.ArrowLeft){
                    scroll++;
                    scrollList(contentPanel4_1);
                }
                
            }

            @Override
            public void onUnhandledInput(Window basePane, KeyStroke keyStroke, AtomicBoolean hasBeenHandled) {
                
                
            }

            @Override
            public void onResized(Window window, TerminalSize oldSize, TerminalSize newSize) {
                try {
                    Client.getWindow().setPosition(new TerminalPosition(Client.getTerminal().getTerminalSize().getColumns() / 2 - Client.getWindow().getSize().getColumns() / 2, 1));
                    maxY = newSize.getRows() - 6;
                } catch (IOException e) {
                    e.printStackTrace();
                }
                
            }

            @Override
            public void onMoved(Window window, TerminalPosition oldPosition, TerminalPosition newPosition) {
            
                
            }
            
        });
    }
    public static void scrollList(Panel panel){
        if(arrayList.size() < maxY) return;

        int maxScroll = arrayList.size() - maxY;
        if(scroll > maxScroll) scroll = maxScroll;
        if(scroll < 0) scroll = 0;

        panel.removeAllComponents();
        for(int y = scroll, y1 = 0; y1 < maxY; y++, y1++){
            if(y > arrayList.size()-1) break;
            panel.addComponent(arrayList.get(y));
        }
    }

    public static void showMess(String mess){
        Label label = new Label(mess);
        label.setLayoutData(GridLayout.createHorizontallyFilledLayoutData(1));
        arrayList.add(label);
        contentPanel4_1.addComponent(label); 
        // currPos++;
        // scroll++;
       // scrollList(contentPanel4_1);
    }

    public static MessageDialogButton showMessageDialog(
        WindowBasedTextGUI textGUI,
        String title,
        String text,
        MessageDialogButton... buttons) {
        MessageDialogBuilder builder = new MessageDialogBuilder()
                .setTitle(title)
                .setText(text);
        if(buttons.length == 0) {
            builder.addButton(MessageDialogButton.OK);
        }
        for(MessageDialogButton button: buttons) {
            builder.addButton(button);
        }
    return builder.build().showDialog(textGUI);
    }
}