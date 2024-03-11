# class Message:
#     # store message structure and handle message creation
#     pass


# class Trade:
#     # use to handle trades
#     # stores the progress and interactions of users
#     # using this will not stop server when waiting for a response
#     pass


import socket
import threading
import pymongo
from enum import Enum


db_client = pymongo.MongoClient("mongodb+srv://farkaspeter2001:T5YY8ln4FJSVg1Ok@indivproj.4twcz60.mongodb.net/")
db = db_client['IndivProj']

active_trade = []


#########################################
# Function to validate user             #
#########################################
def validate_user(message):
    user = dict(db.Users.find_one({"username": message[0]}))
    if (user['password'] == message[1]):
        return "VALID\n" + user['_id']
    return "INVALID"
#########################################
# Function end                          #
#########################################


#########################################
# Function to handle client connections #
#########################################
def handle_client(client_socket, addr):
    print(f"[NEW CONNECTION] {addr} connected.")

    while True:
        message = client_socket.recv(1024).decode('utf-8')
        if not message:
            break  

        print(f"[{addr}] {message}")
        
        response = None

        # split message into command (first line) and message lines
        messageLines = message.splitlines()
        firstLine = messageLines[0].split()
        
        # handle command
        if (firstLine[0] == "VALIDATE"):
            if (firstLine[1] == "USER"):
                response = validate_user(messageLines[1:])
            
        # Echo back the received data
        client_socket.send(bytes(response, 'utf-8'))

    # Close the connection when done
    client_socket.close()
    print(f"[DISCONNECTED] {addr} disconnected.")
#########################################
# Function end                          #
#########################################


# Server configuration
HOST = '127.0.0.1'  # localhost
PORT = 20111

# Create a socket object
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Bind the socket to the host and port
server_socket.bind((HOST, PORT))

# Listen for incoming connections
server_socket.listen()

print(f"[LISTENING] Server is listening on {HOST}:{PORT}")

while True:
    # Accept incoming connection
    client_socket, addr = server_socket.accept()

    # Create a new thread to handle the client
    client_thread = threading.Thread(target=handle_client, args=(client_socket, addr))
    client_thread.start()