package chat;

import java.io.Serializable;
import java.util.ArrayList;

public class UserDialog implements Serializable{
    public String title;
    //private String password;
    public byte numberOfUser;
    public String creationTime;
    
    public ArrayList<ClientController> numbers = new ArrayList<>();
    public UserDialog(){}
    
    public UserDialog(String title, byte numberOfUsers, String creationTime){
        this.title = title;
        this.creationTime = creationTime;
    }
}
