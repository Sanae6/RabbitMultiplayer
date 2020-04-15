var netinst = instance_nearest(0,0,obj_net);
ds_menu_net = create_menu_page(
    // Text input menu option specs:
    // Name, Type (11), Object, Variable Name (string), Max Length, Has character validation[, Characters to allow]
    ["CONNECT", 0, menu_connect], 
    ["IP", 11, netinst, "url", 100, false], 
    ["PORT", 11, netinst, "port", 5, true, "123456789"],
    ["NAME", 11, netinst, "username", 20, false],
    ["BACK", 1, 0]
    //["EDITOR", 0, start_editorbeta]
);