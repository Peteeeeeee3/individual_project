# class Message:
#     # store message structure and handle message creation
#     pass


# class Trade:
#     # use to handle trades
#     # stores the progress and interactions of users
#     # using this will not stop server when waiting for a response
#     pass


import queue
import socket
import threading
import pymongo
from enum import Enum


db_client = pymongo.MongoClient("mongodb+srv://farkaspeter2001:T5YY8ln4FJSVg1Ok@indivproj.4twcz60.mongodb.net/")
db = db_client['IndivProj']

active_trade = []

messageQueue = queue.Queue()


#########################################
# Function to validate user             #
#########################################
def validate_user(message):
    tempUserContainer = db.Users.find_one({"username": message[0]})
    print(tempUserContainer)
    if not tempUserContainer == None:
        user = dict(tempUserContainer)
        if (user['password'] == message[1]):
            return "VALID USER\n" + str(user['_id'])
        
    return "INVALID USER"
#########################################
# Function end                          #
#########################################


#########################################
# Function to handle incoming messages  #
#########################################
def handle_client(client_socket, addr):
    print(f"[NEW CONNECTION] {addr} connected.")

    while True:
        message = client_socket.recv(1024).decode('utf-8')
        if not message:
            break  

        messageQueue.put(message)
        print(f"[{addr}] {message}")

        print(messageQueue)
#########################################
# Function end                          #
#########################################
    

#########################################
# Function to remove 0 width blank space#
#########################################
def remove_ZWBS(text_array):
    for i, text in enumerate(text_array):
        string_encode = text.encode("ascii", "ignore")
        string_decode = string_encode.decode()
        text_array[i] = string_decode
#########################################
# Function end                          #
#########################################


#########################################
# Function to handle message responses  #
#########################################
def handle_response(client_socket):
    while True:
        message = messageQueue.get()
        response = None

        # split message into command (first line) and message lines
        messageLines = message.splitlines()
        print(messageLines)
        remove_ZWBS(messageLines)
        
        firstLine = messageLines[0].split()
        print(firstLine)

        # handle command
        if (firstLine[0] == "VALIDATE"):
            print("validate")
            if (firstLine[1] == "USER"):
                print("user")
                response = validate_user(messageLines[1:])
                print("validated " + response)
            
        # Send response
        client_socket.send(bytes(response, 'utf-8'))
#########################################
# Function end                          #
#########################################


# Server configuration
HOST = '0.0.0.0'  # localhost
PORT = 20111

# Create a socket object
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

print(HOST)
# Bind the socket to the host and port
server_socket.bind((HOST, PORT))

# Listen for incoming connections
server_socket.listen()

print(f"[LISTENING] Server is listening on {HOST}:{PORT}")

while True:
    # Accept incoming connection
    client_socket, addr = server_socket.accept()

    # Thread to handle the client's messages
    client_thread = threading.Thread(target=handle_client, args=(client_socket, addr))
    client_thread.start()

    # Thread to handle responses
    response_thread = threading.Thread(target=handle_response, args=([client_socket]))
    response_thread.start()