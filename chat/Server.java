package chat;

import java.io.IOException;
import java.net.ServerSocket;


public class Server{
    private static ServerSocket server = null;

    public static void main(String[] a) throws IOException, ClassNotFoundException{
        System.out.print("\033[H\033[2J");
        serverInit((short)4444);
    }

    private static void serverInit(short sPort){
        try{
            server = new ServerSocket(sPort);
            System.out.println("-The server is running and waiting for clients :D\n");

            while (!server.isClosed()){
                ClientController mainRoom = new ClientController(server.accept());
                System.out.println(Color.ANSI_CYAN + "-Log: " + Color.ANSI_RESET + mainRoom.nickname + " was connected!");
                Thread th = new Thread(mainRoom);
                th.start();
            }
        }
        catch (IOException e){
            e.printStackTrace();
        }
    }
}
