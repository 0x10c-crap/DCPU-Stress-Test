#!/bin/bash
for i in `cd ../../ && ls *.dasm` 
do
    time ./dtasm ../../$i -o $i.o > /dev/null 2>&1
done
