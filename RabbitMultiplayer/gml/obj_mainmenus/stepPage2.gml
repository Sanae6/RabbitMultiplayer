        case 11:
            if (heylois || inputting) break;
            keyboard_lastkey = -1;
            inputting = true;
            var yy=global.menu_option[global.page];
            cursorpos = string_length(variable_instance_get(ds_grid_get(ds_grid, 2, yy), ds_grid_get(ds_grid, 3, yy)));
            break;