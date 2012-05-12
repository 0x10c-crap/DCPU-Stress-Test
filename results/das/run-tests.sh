#!/bin/sh
# script to run das against SirCmpwn's stress tests
# do little-endian to vaguely agree with organic
# different for now after a while, due to das doing short literals.

for i in `cd ../..; ls *.dasm`
do
	das -o $i.bin --dumpfile $i.dump.txt ../../$i --le >$i.console-out.txt 2>&1 || echo $i failed
done
