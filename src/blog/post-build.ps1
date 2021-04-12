# npm install uglify-js -g
# npm install uglifycss -g
# npm install -g sass
# while (1) { .\build-static-win.ps1; sleep 5 }

$sourceBaseDir = "./"
$blogBaseDir = "$sourceBaseDir/wwwroot"
$blogDistDir = "$blogBaseDir/dist"
$blogLibDir = "$blogBaseDir/../lib"

$blogStyleRoot = "$blogBaseDir/style"
$blogScriptRoot = "$blogBaseDir/script"

sass "$blogStyleRoot/index.scss" "$blogDistDir/index.css"
# sass --load-path="$blogBaseDir/wwwroot" "$blogStyleRoot/post.scss" "$blogDistDir/post.css"
# sass "$blogStyleRoot/archive.scss" "$blogDistDir/archive.css"
# sass "$blogStyleRoot/account.scss" "$blogDistDir/account.css"
# sass "$blogStyleRoot/admin.scss" "$blogDistDir/admin.css"

"scss files converted to CSS."

uglifycss --expand-vars "$blogDistDir/index.css" > "$blogDistDir/index.min.css"
# uglifycss --expand-vars "$blogDistDir/post.css" > "$blogDistDir/post.min.css"
# uglifycss --expand-vars "$blogDistDir/archive.css" > "$blogDistDir/archive.min.css"
# uglifycss --expand-vars "$blogDistDir/account.css" > "$blogDistDir/account.min.css"
# uglifycss --expand-vars "$blogDistDir/admin.css" > "$blogDistDir/admin.min.css"

"All css files are combined and minified."

# uglifyjs `
# "$blogLibDir/ga-lite.js" `
# "$blogScriptRoot/index.js" `
# -o "$blogDistDir/index.min.js"

# "Index js files are combined and minified."

# uglifyjs `
# "$blogDistDir/index.min.js" `
# "$blogLibDir/anchor-js/anchor.min.js" `
# -o "$blogDistDir/archive.min.js"

# "Archive js files are combined and minified."

# uglifyjs `
# "$blogLibDir/highlight/highlight.pack.js" `
# "$blogLibDir/anchor-js/anchor.min.js" `
# "$blogDistDir/index.min.js" `
# "$blogScriptRoot/post.js" `
# -o "$blogDistDir/post.min.js"

# "Post js files are combined and minfied."

# uglifyjs `
# "$blogLibDir/MathJax/es5/tex-svg.js" `
# "$blogDistDir/post.min.js" `
# -o "$blogDistDir/post-mathjax.min.js"

# "Post js files(with mathjax) are combined and minified."

# uglifyjs `
# "$blogScriptRoot/index.js" `
# "$blogLibDir/signalr/signalr.min.js" `
# "$blogScriptRoot/admin.js" `
# -o "$blogDistDir/admin.min.js"

"Admin js files are combined and minified."