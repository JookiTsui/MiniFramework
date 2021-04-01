#!/bin/sh


filelist=`ls /Users/jookitsui/U3DProjects/UnityNet/Assets/Scripts/Proto/*.proto`
for file in $filelist
do
echo $file
protogen -i:$file -o:${file%.*}.cs
done
