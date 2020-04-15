var buffer = setup_buffer();
buffer_write(buffer,buffer_u8,0);
buffer_write(buffer,buffer_string,name);
buffer_write(buffer,buffer_f32,0);//x
buffer_write(buffer,buffer_f32,0);//y
buffer_write(buffer,buffer_u32,room);//x
send_message(0,buffer);