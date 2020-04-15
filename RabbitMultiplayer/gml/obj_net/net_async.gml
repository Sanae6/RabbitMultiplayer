switch(async_load[?"type"]){
    case network_type_non_blocking_connect:
        wop_connect(username);
        knewaboutdisconnect = false;
        break;
    case network_type_disconnect:
        //handle disconnect stuff, clearing fake players
        if (!knewaboutdisconnect) updatetextpop("The connection to the server was unexpectedly closed.",0xadd8e6,12,0);
    case network_type_data:
        var b = async_load[?"buffer"];
        var length = buffer_read(b,buffer_u16);
        var op = buffer_read(b,buffer_u8);
        var pkt = packets[op];
        script_execute(pkt[0],b,length);
        break;
}