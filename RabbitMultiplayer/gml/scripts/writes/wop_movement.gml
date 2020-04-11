var buffer = setup_buffer();
buffer_write(buffer,buffer_f32,argument0);//x
buffer_write(buffer,buffer_f32,argument0);//y
send_message(0,buffer);