case 9:
    current_val = variable_instance_get(ds_grid_get(ds_grid, 2, yy), ds_grid_get(ds_grid, 3, yy));//current text value
    if (keyboard_check_pressed(vk_backspace)){
        current_val = string_copy(current_val,1,string_length(current_val)-1);
    }
    
    break;