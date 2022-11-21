#!/bin/bash

if [[ $1 == "Server" ]]
then
  echo "Compiling the server..."
  javac -d javac/server Server.java ClientController.java Color.java Packet.java TypePacket.java UserDialog.java
  cd javac/server
  jar -cfvm Server.jar MAINFEST-SR.txt chat/
  if [[ $2 != "--log" ]]
  then
  java -jar Server.jar
  fi
elif [[ $1 == "Client" ]]
then
  echo "Compiling the client..."
  javac -d javac/chat -cp lib/lanterna-3.1.1.jar; Client.java ClientController.java UserClient.java Color.java Packet.java TypePacket.java UserDialog.java UIController.java
  cd javac/chat
  jar -cfvm Client.jar MAINFEST-CL.txt chat/
  if [[ $2 != "--log" ]]
  then
  java -jar Client.jar 
  fi
else
  echo "Error"
fi
