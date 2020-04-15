socket = network_create_socket(network_socket_tcp);
network_set_config(network_config_use_non_blocking_socket, 1);
alreadyStarting = false;
connected = false;
username = "Sanae";
url = "localhost";
port = "7773"
version = 1;
global._net = id;
packets = [
    [rop_connect,wop_connect],
    [rop_disconnect,wop_disconnect],
    [rop_movement,wop_movement],
    [rop_playerconnect,wop_playerconnect]
]

var b = buffer_create(1,buffer_grow,1);
buffer_write(b, buffer_string, "Hello");
buffer_save(b, "testbuf.bin")
buffer_delete(b);