socket = network_create_socket(network_socket_tcp);
network_set_config(network_config_use_non_blocking_socket, true);
name = "Sanae";
url = "localhost";//delete this later
port = 7773//delete this tooo
global._net = self
packets = [
    [rop_connect,wop_connect],
    [rop_disconnect,wop_disconnect],
    [rop_movement,wop_movement],
    [rop_playerconnect,wop_playerconnect]
]