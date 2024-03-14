FROM python:3
WORKDIR /server
ADD . /server

RUN apt-get -y update
RUN apt-get -y upgrade
RUN python -m pip install -r requirements.txt

ENV PYTHONUNBUFFERED=1

EXPOSE 20111

CMD ["python", "Server.py"]