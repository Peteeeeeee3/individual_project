# https://www.rfc-editor.org/rfc/rfc4122#section-4.1.3 at 4.2.1.2 using system clock from ID generation
import time

IDs = []

# generate 6 IDs, one for each figure
for i in range(6):
    # use current Unix timestamp in milliseconds to generate unique ID (will never be duplicate)
    timestamp = time.time() * 1000
    print(timestamp)
    # remove decimal values as such accuracy is not needed
    timestamp = int(timestamp)
    print(timestamp)

    # add identifying prefix to ID for application to know whether the ID is intended for its use
    prefix = "NFC-GAME-FIGURE-"
    figure_ID = prefix + str(timestamp)
    print(figure_ID)

    IDs.append(figure_ID)
    time.sleep(1)

print(IDs)