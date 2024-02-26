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


db_client = pymongo.MongoClient("mongodb+srv://farkaspeter2001:T5YY8ln4FJSVg1Ok@indivproj.4twcz60.mongodb.net/")
db = db_client['IndivProj']

active_trade = []


# Function to handle client connections
def handle_client(client_socket, addr):
    print(f"[NEW CONNECTION] {addr} connected.")

    while True:
        # Receive data from the client
        data = client_socket.recv(1024).decode('utf-8')
        if not data:
            break  # If no data received, break the loop

        print(f"[{addr}] {data}")

        # Echo back the received data
        client_socket.send(bytes(data, 'utf-8'))

    # Close the connection when done
    client_socket.close()
    print(f"[DISCONNECTED] {addr} disconnected.")

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