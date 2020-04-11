var buffer = setup_buffer();
buffer_write(buffer,buffer_u16,global.plid);
send_message(0,buffer);