import socket

# Server configuration
SERVER_HOST = '127.0.0.1'  
SERVER_PORT = 20111        

# Create a socket object
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Connect to the server
client_socket.connect((SERVER_HOST, SERVER_PORT))
print("[CONNECTED] Connected to the server.")

while True:
    # Get user input
    message = input("Enter a message to send to the server (type 'quit' to exit): ")

    if message.lower() == 'quit':
        break

    # Send the message to the server
    client_socket.send(bytes(message, 'utf-8'))

    # Receive a response from the server
    response = client_socket.recv(1024).decode('utf-8')
    print(f"[RECEIVED] Server response: {response}")

# Close the connection
client_socket.close()
print("[DISCONNECTED] Disconnected from the server.")