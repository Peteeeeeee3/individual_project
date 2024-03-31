# class Message:
#     # store message structure and handle message creation
#     pass


# class Trade:
#     # use to handle trades
#     # stores the progress and interactions of users
#     # using this will not stop server when waiting for a response
#     pass


import queue, socket, threading, pymongo, bson
from enum import Enum


db_client = pymongo.MongoClient("mongodb+srv://farkaspeter2001:T5YY8ln4FJSVg1Ok@indivproj.4twcz60.mongodb.net/")
db = db_client['IndivProj']

active_trade = []

messageQueue = queue.Queue()


#!#######################################
#! Function to validate user            #
#!#######################################
def validate_user(message):
    tempUserContainer = db.Users.find_one({"username": message[0]})
    print(tempUserContainer)
    if not tempUserContainer == None:
        user = dict(tempUserContainer)
        if (user['password'] == message[1]):
            return "VALID USER\n" + str(user['_id'])
        
    return "INVALID USER"
#?#######################################
#? Function end                         #
#?#######################################


#!#######################################
#! Function to handle incoming messages #
#!#######################################
def handle_client(client_socket, addr):
    print(f"[NEW CONNECTION] {addr} connected.")
    print("receiving thread " + str(messageQueue))

    while True:
        message = client_socket.recv(1024).decode('utf-8')
        if not message:
            break  

        messageQueue.put(message)
        print(f"[{addr}] {message}")

        print(messageQueue)
#?#######################################
#? Function end                         #
#?#######################################
    

#!#######################################
#!Function to remove 0 width blank space#
#!#######################################
def remove_ZWBS(text_array):
    for i, text in enumerate(text_array):
        string_encode = text.encode("ascii", "ignore")
        string_decode = string_encode.decode()
        text_array[i] = string_decode
#?#######################################
#? Function end                         #
#?#######################################


#!#######################################
#!Function to handle getting all figures#
#!#######################################
def get_all_figures(message):
    tempUserContainer = db.Users.find_one({"_id": bson.ObjectId(message[0])})
    if not tempUserContainer == None:
        user = dict(tempUserContainer)
        response = "SUCCESS FIGURES\n"
        for figure in user['figures']:
            response +=\
                figure['_id'] + "\n" +\
                figure['type'] + "\n" +\
                str(figure['level']) + "\n" +\
                str(figure['available-upgrades']) + "\n" +\
                str(figure['exp']) + "\n" +\
                str(figure['move-speed']) + "\n" +\
                str(figure['damage']) + "\n" +\
                str(figure['attack-rate']) + "\n" +\
                str(figure['attack-range']) + "\n"
            # edit the formatting once figure data is finalised

        return response

    return "ERROR FIGURES"
#?#######################################
#? Function end                         #
#?#######################################


#!#######################################
#! Function to handle getting a figure  #
#!#######################################
def get_figure(message):
    tempUserContainer = db.Users.find_one({"_id": bson.ObjectId(message[1])})
    if not tempUserContainer == None:
        user = dict(tempUserContainer)
        for figure in user['figures']:
            if str(figure['_id']) == message[2]:
                return "FIGURE FOUND\n" +\
                message[0] + "\n" +\
                figure['_id'] + "\n" +\
                figure['type'] + "\n" +\
                str(figure['level']) + "\n" +\
                str(figure['available-upgrades']) + "\n" +\
                str(figure['exp']) + "\n" +\
                str(figure['move-speed']) + "\n" +\
                str(figure['damage']) + "\n" +\
                str(figure['attack-rate']) + "\n" +\
                str(figure['attack-range']) + "\n"

    return "FIGURE NOT FOUND"                
#?#######################################
#? Function end                         #
#?#######################################


#!#######################################
#! Function to handle updating a figure #
#!#######################################
def update_figure(message):
    tempUserContainer = db.Users.find_one({"_id": bson.ObjectId(message[0])})
    if not tempUserContainer == None:
        user = dict(tempUserContainer)
        for figure in user['figures']:
            if figure['_id'] == message[1]:
                figure['level'] = message[3]
                figure['exp'] = message[4]
                print(figure)

        print(user['figures'])
        db.Users.update_one({"_id": bson.ObjectId(message[0])}, {"$set": {"figures": user['figures']}})
        return "FIGURE UPDATED"

    return "FAILED TO UPDATE FIGURE"
#?#######################################
#? Function end                         #
#?#######################################


#!#######################################
#! Function to handle updating a stat   #
#!#######################################
def upgrade_stat(message):
    tempUserContainer = db.Users.find_one({"_id": bson.ObjectId(message[0])})
    if not tempUserContainer == None:
        user = dict(tempUserContainer)
        for figure in user['figures']:
            if figure['_id'] == message[1]:
                if message[2] == "MOVE SPEED":
                    figure['move-speed'] = int(message[3])
                
                elif message[2] == "DAMAGE":
                    figure['damage'] = int(message[3])
                
                elif message[2] == "ATTACK RATE":
                    figure['attack-rate'] = float(message[3])
                
                elif message[2] == "ATTACK RANGE":
                    figure['attack-range'] = float(message[3])
                    
                figure['available-upgrades'] -= 1
                db.Users.update_one({"_id": bson.ObjectId(message[0])}, {"$set": {"figures": user['figures']}})
                return "SUCCESSFULLY UPDATED ATTACK RANGE STAT"
                
    return "FAILED TO UPDATE STAT"
#?#######################################
#? Function end                         #
#?#######################################


#!#######################################
#! Function to register a figure        #
#!#######################################
def register_figure(message):
    tempUserContainer = db.Users.find_one({"_id": bson.ObjectId(message[0])})
    if not tempUserContainer == None:
        user = dict(tempUserContainer)
        # check first if user already has this figure
        for figure in user['figures']:
            if figure['_id'] == message[1]:
                return "FIGURE ALREADY OWNED"
            
        # check whether figure exists in available db
        tempFigureContainer = db.UnassignedFigures.find_one({"ID": message[1]})
        if not tempFigureContainer == None:
            figure = dict(tempFigureContainer)
            numFigures = len(user['figures'])
            convertedFigure = {
                "_id": figure['ID'],
                "type": figure['type'],
                "attack-range": figure['attack-range'],
                "attack-rate": figure['attack-rate'],
                "damage": figure['damage'],
                "exp": figure['exp'],
                "level": figure['level'],
                "move-speed": figure['move-speed'],
                "available-upgrades": figure['available-upgrades']
            }
            user['figures'].insert(numFigures, convertedFigure)

            db.Users.update_one({"_id": bson.ObjectId(message[0])}, {"$set": {"figures": user['figures']}})
            db.UnassignedFigures.delete_one({"ID": message[1]})
            return "FIGURE REGISTERED"

    return "FIGURE NOT REGISTERED"
#?#######################################
#? Function end                         #
#?#######################################


#!#######################################
#! Function to handle getting a user    #
#!#######################################
def register_user(message):
    tempUserContainer = db.Users.find_one({"username": message[0]})
    print(tempUserContainer)
    if tempUserContainer == None:
        userId = bson.ObjectId()
        db.Users.insert_one({"_id": userId, "username": message[0], "password": message[1], "figures": []})
        return "USER REGISTERED"
    
    return "USERNAME IN USE"           
#?#######################################
#? Function end                         #
#?#######################################


#!#######################################
#! Function to handle message responses #
#!#######################################
def handle_response(client_socket):
    while True:
        if (not messageQueue.empty()):
            message = messageQueue.get()
            response = None

            # split message into command (first line) and message lines
            messageLines = message.splitlines()
            print(messageLines)
            remove_ZWBS(messageLines)
            print(messageLines)
            
            firstLine = messageLines[0].split()
            print(firstLine)

            # handle command
            if firstLine[0] == "VALIDATE":
                print("validate")
                if firstLine[1] == "USER":
                    print("user")
                    response = validate_user(messageLines[1:])
                    print("validated " + response)

            elif firstLine[0] == "GET":
                print("get")
                if firstLine[1] == "FIGURES":
                    response = get_all_figures(messageLines[1:])
                elif firstLine[1] == "FIGURE":
                    response = get_figure(messageLines[1:])
            
            elif firstLine[0] == "UPDATE":
                print("update")
                if firstLine[1] == "FIGURE":
                    response = update_figure(messageLines[1:])
                elif firstLine[1] == "STAT":
                    response = upgrade_stat(messageLines[1:])

            elif firstLine[0] == "REGISTER":
                if firstLine[1] == "FIGURE":
                    response = register_figure(messageLines[1:])
                elif firstLine[1] == "USER":
                    response = register_user(messageLines[1:])

            if response == None:
                continue    

            # Send response
            print("Response => " + response)
            client_socket.send(bytes(response, 'utf-8'))
            print("Reponse sent!")
#?#######################################
#? Function end                         #
#?#######################################


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