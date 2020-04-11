with(global._net){
    //if (!variable_instance_exists(global.,"url") || !variable_instance_exists(self,"port") || name == ""){
    //    updatetextpop("Enter the IP, port, and your username.",0xff0000,6,0);
    //    exit;
    //
    }
    if (alreadyStarting) {
        updatetextpop("Already connecting...",0xadd8e6,12,0);
        exit;
    }
    network_connect_raw(socket,url,port);
    updatetextpop("Connecting...",0xadd8e6,12,0);
}