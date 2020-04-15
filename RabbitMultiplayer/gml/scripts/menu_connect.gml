with(global._net){
    if (url == "" || port == "" || username == ""){
        updatetextpop("Enter the IP, port, and your username.",0xff0000,6,0);
        exit;
    }
    if (alreadyStarting) {
        if (connected){
            wop_disconnect("Attempting to reconnect");
            network_destroy(socket);
            connected = false;
        }
        network_destroy(socket);
        updatetextpop("Reattempting a connection...",0xadd8e6,12,0);
        socket = network_create_socket(network_socket_tcp);
    }
    network_connect_raw(socket,url,real(port));
    //if (!alreadyStarting)updatetextpop("Connecting...",0xadd8e6,12,0);
    alreadyStarting = true;
}