is_app_closed=false

declare services=( "dhcp" )
declare entry_points=( "app_main" "dhcp_main" )
declare c_entry_points=( "./macros/so/parser.so" )

function app_main {
    app_list_of_services
}

function app_list_of_services {
    local indexing=0

    clear
    echo "Wybierz jaki serwis będziesz konfigurować:"
    for key in "${!services[@]}"
    do
        echo "$((++indexing)). ${services[$key]}"
    done
    "${entry_points[$(app_print)]}"
}

function app_print {
    local result=""

    read -p "> " result
    echo "$result"
}

function app_replace_line {
    local file=$3
    local line_number=$1
    local new_line=$2

    sed -i "${line_number}s/.*/${new_line}/" "$file"
}

function app_delete_line {
    local file=$2
    local line_number=$1

    sed -i "${line_number}d" "$file"
}

function app_append_line {
    local file=$3
    local line_number=$1
    local new_line=$2

    sed -i "${line_number}i${new_line}" "$file"
}

function app_get_line {
    local file=$2
    local line_number=$1

    sed -n "${line_number}p" "$file"
}

function app_verify {
    local file=$1
    local count=$(grep -c "#\*approved\* \"\"" $file)

    if [[ $count -lt 1 ]]; then
        echo -e "#*approved* \"\"\n" > $file
    fi
}   
