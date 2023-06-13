#!/bin/bash

source reference.cfg

UNETTOOLS="unettools.dll"
echo -e "===================================\nCompiling \033[1;31m$UNETTOOLS\033[0m"

csc /target:library /out:$O_OUT$UNETTOOLS /reference:"$R_MSCORLIB,$R_NETSTANDART,$R_UNITYENGINE" $CT_UNWLOGS $CT_UNWUPDATE $CT_UNWPACKETTOOLS

echo -e "\nCompilation of \033[1;32m$UNETTOOLS\033[0m is complete \n==================================="

UNETCLIENT="unetclient.dll"
echo -e "===================================\nCompiling \033[1;31m$UNETCLIENT\033[0m"

csc /target:library /out:$O_OUT$UNETCLIENT /reference:"$R_MSCORLIB,$R_NETSTANDART,$R_SYSTEM,$R_UNITYEDITOR,$R_UNITYENGINE,$O_OUT/unettools.dll" $CC_UNWCORE $CC_UNWMCLIENT $CC_UNWIORULES

echo -e "\nCompilation of \033[1;32m$UNETCLIENT\033[0m is complete \n==================================="

UNETSERVER="unetserver.dll"
echo -e "===================================\nCompiling \033[1;31m$UNETSERVER\033[0m"

csc /target:library /out:$O_OUT$UNETSERVER /reference:"$R_MSCORLIB,$R_NETSTANDART,$R_SYSTEM,$R_UNITYEDITOR,$R_UNITYENGINE,$O_OUT/unettools.dll" $CS_UNWCLIENT $CS_UNWCORE $CS_UNWMSERVER $CS_UNWIORULES
echo -e "\nCompilation of \033[1;32m$UNETSERVER\033[0m is complete \n==================================="

