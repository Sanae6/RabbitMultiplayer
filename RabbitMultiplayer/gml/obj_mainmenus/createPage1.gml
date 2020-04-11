ds_menu_net = create_menu_page(
    // Text input menu option specs:
    // Name, Type (9), Object, Variable Name (string), Max Length, Has character validation[, Characters to allow]
    ["CONNECT", 0, menu_connect], 
    ["IP", 9, obj_net, "url", 15, true, "123456789."], //true means it only inputs whatever you place
    ["PORT", 9, obj_net, "port", 5, true, "123456789"],
    ["NAME", 9, obj_net, "username", 20, false],//false means anything you want can be placed into the field
    //except for "|" because i'll use that as the cursor lol
    ["BACK", 1, 0]
);