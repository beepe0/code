declare tools=( 
    "Uaktywnić serwer DHCP dla interfejsu v4" 
    "Uaktywnić serwer DHCP dla interfejsu v6" 
    "Uaktywnić funkcję DDNS dla serwera DHCP" 
    "Uaktywnić serwer DHCP jako autorytatywny" 
    "Dodać nową sieć" 
    "Dodać do sieci zakres adresów" 
    "Dodać do sieci bramę domyślną" 
    "Dodać do sieci wyjątek" 
    "Dodać do wyjątka MAC (identyfikator)" 
    "Dodać do wyjątka rezerwacji adresu" 
)

declare dhcp_entry_points=( 
    "dhcp_active_v4" 
    "dhcp_active_v6" 
    "dhcp_active_ddns" 
    "dhcp_active_authoritative" 
    "dhcp_add_subnet" 
    "dhcp_add_range" 
    "dhcp_add_gate" 
    "dhcp_add_exception" 
    "dhcp_add_mac"
    "dhcp_add_fixed_address"
)

dhcp_link_to_isc_dhcp_server=""
dhcp_link_to_dhcpd_conf=""

function dhcp_main {
    dhcp_update_links_to_files
    dhcp_list_of_tools
}

function dhcp_update_links_to_files {
    dhcp_link_to_isc_dhcp_server=$DHCP_ISC_DHCP_SERVER
    dhcp_link_to_dhcpd_conf=$DHCP_DHCPD_CONF
}

function dhcp_list_of_tools {
    clear
    echo "Wybierz co będziesz konfigurować:"

    local indexing=0
    for key in "${!tools[@]}"
    do
        echo "$((indexing++)). ${tools[$key]}"
    done
    
    "${dhcp_entry_points[$(app_print)]}"
}

function dhcp_active_v4 {
    clear

    app_verify $dhcp_link_to_isc_dhcp_server

    echo "Wpisz interfejs karty sieciowej dla wersji 4: "
    local v4=$(app_print)

    if [[ -z "$v4" ]]; then
        return
    fi

    local index=1
    local count=$(grep -c "INTERFACESv4" $dhcp_link_to_isc_dhcp_server)
       
    while IFS= read -r line
    do
        if [[ -n "$line" ]]; then
            if [[ $count -gt 0 ]]; then 
                if [[ -n "$(echo $line | grep -m 1 "INTERFACESv4")" ]]; then
                    app_replace_line $((index)) "INTERFACESv4=\"$v4\"" "$dhcp_link_to_isc_dhcp_server"
                    break
                fi
            elif [[ $count -lt 1 ]]; then
                app_append_line $((index+1)) "INTERFACESv4=\"$v4\"" "$dhcp_link_to_isc_dhcp_server"
                break
            fi
        fi
        index=$((index + 1))
    done < $dhcp_link_to_isc_dhcp_server
}

function dhcp_active_v6 {
    clear

    app_verify $dhcp_link_to_isc_dhcp_server

    echo "Wpisz interfejs karty sieciowej dla wersji 6: "
    local v6=$(app_print)
    
    if [[ -z "$v6" ]]; then
        return
    fi

    local index=1
    local count=$(grep -c "INTERFACESv6" $dhcp_link_to_isc_dhcp_server)
       
    while IFS= read -r line
    do
        if [[ -n "$line" ]]; then
            if [[ $count -gt 0 ]]; then 
                if [[ -n "$(echo $line | grep -m 1 "INTERFACESv6")" ]]; then
                    app_replace_line $((index)) "INTERFACESv6=\"$v6\"" "$dhcp_link_to_isc_dhcp_server"
                    break
                fi
            elif [[ $count -lt 1 ]]; then
                app_append_line $((index+1)) "INTERFACESv6=\"$v6\"" "$dhcp_link_to_isc_dhcp_server"
                break
            fi
        fi
        index=$((index + 1))
    done < $dhcp_link_to_isc_dhcp_server
}

function dhcp_active_ddns {
    clear

    app_verify $dhcp_link_to_dhcpd_conf

    echo "Wpisz tryb pracy ddns (ad-hoc, interim, standard, none): "
    local mode_ddns=$(app_print)
    
    if [[ -z "$mode_ddns" ]]; then
        return
    fi

    local index=1
    local count=$(grep -c "ddns-update-style" $dhcp_link_to_dhcpd_conf)
       
    while IFS= read -r line
    do
        if [[ -n "$line" ]]; then
            if [[ $count -gt 0 ]]; then 
                if [[ -n "$(echo $line | grep -m 1 "ddns-update-style")" ]]; then
                    app_replace_line $((index)) "ddns-update-style $mode_ddns;" "$dhcp_link_to_dhcpd_conf"
                    break
                fi
            elif [[ $count -lt 1 ]]; then
                app_append_line $((index+1)) "ddns-update-style $mode_ddns;" "$dhcp_link_to_dhcpd_conf"
                break
            fi
        fi
        index=$((index + 1))
    done < $dhcp_link_to_dhcpd_conf
}

function dhcp_add_subnet {
    clear

    app_verify $dhcp_link_to_dhcpd_conf
    
    echo "Wpisz address sieci:"
    local subnet=$(app_print)
    echo "Wpisz maskę sieci:"
    local netmask=$(app_print)
    
    if [[ -z "$subnet" || -z "$netmask" ]]; then
        return
    fi

    local index=1
    local count=$(grep -c "subnet $subnet" $dhcp_link_to_dhcpd_conf)

    if [[ $count -lt 1 ]]; then
        app_append_line $((index+1)) "subnet $subnet netmask $netmask {\n}" "$dhcp_link_to_dhcpd_conf"
    fi
}

function dhcp_active_authoritative {
    clear

    app_verify $dhcp_link_to_dhcpd_conf

    echo "Uaktywnić? (tak, nie):"
    local mode=$(app_print)

    if [[ -z "$mode" ]]; then
        return
    fi

    local index=1
    local count=$(grep -c "authoritative" $dhcp_link_to_dhcpd_conf)

    while IFS= read -r line
    do
        if [[ -n "$line" ]]; then
            if [[ $count -gt 0 ]]; then 
                if [[ -n "$(echo $line | grep -m 1 "authoritative")" && "$mode" == "nie" ]]; then
                    app_delete_line $((index)) "$dhcp_link_to_dhcpd_conf"
                    break
                fi
            elif [[ $count -lt 1 && "$mode" == "tak" ]]; then
                app_append_line $((index+1)) "authoritative;" "$dhcp_link_to_dhcpd_conf"
                break
            fi
        fi
        index=$((index + 1))
    done < $dhcp_link_to_dhcpd_conf
}

function dhcp_add_range {
    clear

    app_verify $dhcp_link_to_dhcpd_conf

    echo "Wpisz address sieci dla której chcesz dodać zakres adresów:"
    local address=$(app_print)
    echo "Wpisz początkowy address:"
    local start_address=$(app_print)
    echo "Wpisz ostatni address:"
    local end_address=$(app_print)
    
    if [[ -z "$address" || -z "$start_address" || -z "$end_address" ]]; then
        return
    fi

    local index=1
    local count=$(grep -c "#\*range\* $address" $dhcp_link_to_dhcpd_conf)

    while IFS= read -r line
    do
        if [[ -n "$line" ]]; then
            if [[ $count -gt 0 ]]; then 
                if [[ -n "$(echo $line | grep -m 1 "#\*range\* $address")" ]]; then
                    app_replace_line $((index+1)) "range $start_address $end_address;" "$dhcp_link_to_dhcpd_conf"
                    break
                fi
            elif [[ $count -lt 1 && -n "$(echo $line | grep -m 1 "subnet $address")" ]]; then
                app_append_line $((index+1)) "#*range* $address\nrange $start_address $end_address;" "$dhcp_link_to_dhcpd_conf"
                break
            fi
        fi
        index=$((index + 1))
    done < $dhcp_link_to_dhcpd_conf
}

function dhcp_add_gate {
    clear

    app_verify $dhcp_link_to_dhcpd_conf

    echo "Wpisz address sieci dla której chcesz dodać bramę domyślną:"
    local address=$(app_print)
    echo "Wpisz address:"
    local gate_address=$(app_print)
    
    if [[ -z "$address" || -z "$gate_address" ]]; then
        return
    fi

    local index=1
    local count=$(grep -c "#\*option routers\* $address" $dhcp_link_to_dhcpd_conf)

    while IFS= read -r line
    do
        if [[ -n "$line" ]]; then
            if [[ $count -gt 0 ]]; then 
                if [[ -n "$(echo $line | grep -m 1 "#\*option routers\* $address")" ]]; then
                    app_replace_line $((index+1)) "option routers $gate_address;" "$dhcp_link_to_dhcpd_conf"
                    break
                fi
            elif [[ $count -lt 1 && -n "$(echo $line | grep -m 1 "subnet $address")" ]]; then
                app_append_line $((index+1)) "#*option routers* $address\noption routers $gate_address;" "$dhcp_link_to_dhcpd_conf"
                break
            fi
        fi
        index=$((index + 1))
    done < $dhcp_link_to_dhcpd_conf
}

function dhcp_add_exception {
    clear

    app_verify $dhcp_link_to_dhcpd_conf

    echo "Wpisz address sieci dla której chcesz dodać wyjątek:"
    local address=$(app_print)
    echo "Wpisz nazwę wyjątka:"
    local name=$(app_print)
    
    if [[ -z "$address" || -z "$name" ]]; then
        return
    fi

    local index=1
    local count=$(grep -c "#\*host\* $address $name" $dhcp_link_to_dhcpd_conf)

    while IFS= read -r line
    do
        if [[ -n "$line" ]]; then
            if [[ $count -gt 0 ]]; then 
                if [[ -n "$(echo $line | grep -m 1 "#\*host\* $address $name")" ]]; then
                    app_replace_line $((index+1)) "host $name {" "$dhcp_link_to_dhcpd_conf"
                    break
                fi
            elif [[ $count -lt 1 && -n "$(echo $line | grep -m 1 "subnet $address")" ]]; then
                app_append_line $((index+1)) "#\*host\* $address $name\nhost $name {\n}" "$dhcp_link_to_dhcpd_conf"
                break
            fi
        fi
        index=$((index + 1))
    done < $dhcp_link_to_dhcpd_conf
}

function dhcp_add_mac {
    clear

    app_verify $dhcp_link_to_dhcpd_conf

    echo "Wpisz address sieci w której się znajduje wyjątek: "
    local address=$(app_print)
    echo "Wpisz nazwę wyjątka:"
    local name=$(app_print)
    echo "Wpisz MAC hosta:"
    local mac=$(app_print)
    
    if [[ -z "$address" || -z "$name" || -z "$mac" ]]; then
        return
    fi

    local index=1
    local count=$(grep -c "#\*hardware ethernet\* $address $name" $dhcp_link_to_dhcpd_conf)

    while IFS= read -r line
    do
        if [[ -n "$line" ]]; then
            if [[ $count -gt 0 ]]; then 
                if [[ -n "$(echo $line | grep -m 1 "#\*hardware ethernet\* $address $name")" ]]; then
                    app_replace_line $((index+1)) "hardware ethernet $mac;" "$dhcp_link_to_dhcpd_conf"
                    break
                fi
            elif [[ $count -lt 1 && -n "$(echo $line | grep -m 1 "host $name")" ]]; then
                app_append_line $((index+1)) "#\*hardware ethernet\* $address $name\nhardware ethernet $mac;" "$dhcp_link_to_dhcpd_conf"
                break
            fi
        fi
        index=$((index + 1))
    done < $dhcp_link_to_dhcpd_conf
}

function dhcp_add_fixed_address {
    clear

    app_verify $dhcp_link_to_dhcpd_conf

    echo "Wpisz address sieci w której się znajduje wyjątek: "
    local address=$(app_print)
    echo "Wpisz nazwę wyjątka: "
    local name=$(app_print)
    echo "Wpisz address: "
    local fixed_address=$(app_print)
    
    if [[ -z "$address" || -z "$name" || -z "$fixed_address" ]]; then
        return
    fi

    local index=1
    local count=$(grep -c "#\*fixed-address\* $address $name" $dhcp_link_to_dhcpd_conf)

    while IFS= read -r line
    do
        if [[ -n "$line" ]]; then
            if [[ $count -gt 0 ]]; then 
                if [[ -n "$(echo $line | grep -m 1 "#\*fixed-address\* $address $name")" ]]; then
                    app_replace_line $((index+1)) "fixed-address $fixed_address;" "$dhcp_link_to_dhcpd_conf"
                    break
                fi
            elif [[ $count -lt 1 && -n "$(echo $line | grep -m 1 "host $name")" ]]; then
                app_append_line $((index+1)) "#\*fixed-address\* $address $name\nfixed-address $fixed_address;" "$dhcp_link_to_dhcpd_conf"
                break
            fi
        fi
        index=$((index + 1))
    done < $dhcp_link_to_dhcpd_conf
}

