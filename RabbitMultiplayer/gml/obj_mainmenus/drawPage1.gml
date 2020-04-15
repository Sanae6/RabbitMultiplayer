case 11:
    current_val = variable_instance_get(ds_grid_get(ds_grid, 2, yy), ds_grid_get(ds_grid, 3, yy));//current text value
    outc = 0;//black
    c1 = 0xFFFFFF
    if (inputting && (yy == global.menu_option[global.page]))
        outc = 0x333333;//set text color to yellow if selected
    
    draw_text_ext_transformed_color((rtx + 1), rty, current_val, -1, -1, 0.5, 0.5, 0, outc, outc, outc, outc, 1)
    draw_text_ext_transformed_color((rtx - 1), rty, current_val, -1, -1, 0.5, 0.5, 0, outc, outc, outc, outc, 1)
    draw_text_ext_transformed_color(rtx, (rty + 1), current_val, -1, -1, 0.5, 0.5, 0, outc, outc, outc, outc, 1)
    draw_text_ext_transformed_color(rtx, (rty - 1), current_val, -1, -1, 0.5, 0.5, 0, outc, outc, outc, outc, 1)
    draw_text_ext_transformed_color(rtx, rty, current_val, -1, -1, 0.5, 0.5, 0, c1, c1, c1, c1, 1)
    break;