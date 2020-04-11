switch(async_load[?"type"]){
    case network_type_connect:
        op_connect(name);
        
    case network_type_data:
        var b = async_load[?"buffer"];
        var length = buffer_read(b,buffer_u16);
        var op = 
}