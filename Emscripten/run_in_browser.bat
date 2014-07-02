:: Super-awesome syntax gotcha with the start utility:
:: 'start app.html' will run the default browser for app.html, but we want to escape the string app.html if it contains spaces.
:: 'start "app with spaces.html"' will _not_ launch html file, but instead runs a new commandline interpreter and give it the window title "app with spaces.html"
:: 'start "window title" "app with spaces.html"' will run the html file properly escaped. Therefore pass a dummy window title.
@start "Emscripten Browser Run" "%*"
