//setup_buffer()
//var b = buffer_create(3,1,1);
//buffer_seek(b, 0, 3);
//return b;
var b = buffer_create(3,buffer_grow,1);
buffer_seek(b, buffer_seek_start, 3);
return b;