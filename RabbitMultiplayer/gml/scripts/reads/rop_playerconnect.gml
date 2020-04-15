pn = buffer_read(argument0, buffer_string);
var faker = instance_create_layer(buffer_read(argument0, buffer_f32),buffer_read(argument0, buffer_f32), "Instances", obj_faker);
faker.rm = buffer_read(argument0, buffer_u32);
faker.palette = buffer_read(argument0, buffer_u16);