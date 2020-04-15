        case 11:
            var yy=global.menu_option[global.page];
            current_val = variable_instance_get(ds_grid_get(ds_grid, 2, yy), ds_grid_get(ds_grid, 3, yy));//current text value
            if (keyboard_check_pressed(vk_return)) {
                keyboard_lastkey = -1;
                inputting = false;
                heylois = 1;
            }
            if (keyboard_lastkey == vk_backspace){
                current_val = string_copy(current_val,1,string_length(current_val)-1);
                keyboard_lastkey = -1;
                keyboard_lastchar = "";
            }
            if (keyboard_lastkey == vk_tab){
                keyboard_lastchar = "";
                keyboard_lastkey = -1;
                global.menu_option[global.page] = ds_grid_get(ds_grid,1,yy+1) == 11 ? yy+1 : 0;
                if (ds_grid_get(ds_grid,1,yy+1) != 11)inputting = false;
            }
            if (keyboard_lastchar != "" && keyboard_lastkey != -1 && string_length(current_val) < ds_grid_get(ds_grid, 4, yy)){
                if (!ds_grid_get(ds_grid, 5, yy) || string_pos(keyboard_lastchar,ds_grid_get(ds_grid, 6, yy)) != 0)current_val += keyboard_lastchar;
                keyboard_lastkey = -1;
            }
            variable_instance_set(ds_grid_get(ds_grid, 2, yy),ds_grid_get(ds_grid, 3, yy),current_val);
            break;