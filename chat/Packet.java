package chat;

import java.io.Serializable;

public class Packet implements Serializable {
    public byte typePacket = -1;
    public Object bag = null;

    public Packet(byte typePacket, Object bag){
        this.typePacket = typePacket;
        this.bag = bag;
    }
}