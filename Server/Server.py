# echo-server.py

import socket

HOST = "127.0.0.1"  # Standard loopback interface address (localhost)
PORT = 65432  # Port to listen on (non-privileged ports are > 1023)

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST, PORT))
    s.listen()
    conn, addr = s.accept()
    with conn:
        print(f"Connected by {addr}")
        while True:
            data = conn.recv(1024)
            if not data:
                break
            conn.sendall(data)

class Connection:
    # might not need this
    # can likely just use a message backlog to handle requests from multiple clients
    # thinking for this was to have a Connection thread for each actuall connection, but this might not work as intended 
    # and may also be overkill and inefficient
    pass


class Message:
    # store message structure and handle message creation
    pass

class Trade:
    # use to handle trades
    # stores the progress and interactions of users
    # using this will not stop server when waiting for a response
    pass