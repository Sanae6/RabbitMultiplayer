var b = setup_buffer();
buffer_write(b,buffer_u16,version);
send_message(0,b);