const fs = require('fs');
const exec = require('child_process').execSync;

// if log.txt exists, delete it

// for each ip in range 0.0.0.0 - 255.255.255.255
var start = "1.129.0.0";
var end = "255.255.255.255";

// split the ip into table of ints
var start_ip = start.split(".");
var end_ip = end.split(".");

function main(){
    for(var i = start_ip[0]; i <= end_ip[0]; i++) {
        for(var j = start_ip[1]; j <= end_ip[1]; j++) {
            pingRange(i+"."+j);
        }
    }
}
main();

function ping(ip) {
    // ping the ip and return boolean
    try {
        var res = exec("fping -c 1 -b 1 -t 55 " + ip).toString();
        return(res != null);
    } catch (e) {
        return false;
    }
}

function pingRange(ip) {
    var ip_tab = ip.split(".");
    var file = "res/"+ip+".txt";
    console.log(file);
    if(fs.existsSync(file)) {
        fs.unlinkSync(file);
    }
    var i = ip_tab[0];
    var j = ip_tab[1];
    for(var k = 0; k <= 255; k++) {
        for(var l = 0; l <= 255; l++) {
            if(ping(i + "." + j + "." + k + "." + l)) {
                fs.appendFileSync(file, "1");
            } else {
                fs.appendFileSync(file, "0");
            }
        }
        fs.appendFileSync(file, "\n");
    }
}


