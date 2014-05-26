
Sys.Application.add_init(AppInit);

function AppInit(sender) {
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(End);
}

function End(sender, args) { }


