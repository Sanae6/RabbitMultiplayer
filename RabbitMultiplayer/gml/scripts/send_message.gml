///send_message(opcode,buffer)
var buffer = argument1;
var opcode = argument0;
buffer_seek(buffer,buffer_seek_start,0);
buffer_write(buffer,buffer_u16,buffer_get_size(buffer)-2);
buffer_write(buffer,buffer_u8,opcode);
network_send_raw(socket,buffer,buffer_get_size(buffer));
buffer_delete(buffer);