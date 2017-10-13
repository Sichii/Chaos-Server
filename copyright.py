# -*- coding: utf-8 -*-
import os
import re
import fnmatch
import argparse
from textwrap import dedent


parser = argparse.ArgumentParser(description='Add/update copyright on C# files')
parser.add_argument('root', nargs=1, help='C:\Users\Sichi\Documents\Visual Studio 2015\Projects\Chaos-Server')
args = parser.parse_args()

# Descend into the 'root' directory and find all *.cs files
files = [] 
for root, dirnames, filenames in os.walk(args.root[0]):
    for filename in fnmatch.filter(filenames, "*.cs"):
        files.append(os.path.join(root, filename))
print "Found {0} *.cs files".format(len(files))

for filepath in files:
    with open(filepath) as f:
        contents = f.read()

    # This regex will separate the contents of a *.cs file into two parts.
    # The first part is any text that appears before either 'using' or
    # 'namespace' - perhaps an old copyright. The second part *should* be
    # the actual code beginning with 'using' or 'namespace'.
    match = re.search(r"^.*?((using|namespace|/\*|#).+)$", contents, re.DOTALL)
    if match:
        # Make the file's now contain the user defined copyright (below)
        # followed by a blank line followed by the actual code.
        contents = dedent('''\
            // ****************************************************************************
            // This file belongs to the Chaos-Server project.
            // 
            // This project is free and open-source, provided that any alterations or
            // modifications to any portions of this project adhere to the
            // Affero General Public License (Version 3).
            // 
            // A copy of the AGPLv3 can be found in the project directory.
            // You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
            // ****************************************************************************
    
            ''').format(os.path.basename(filepath)) + match.group(1)
        with open(filepath, 'w') as f:
            f.write(contents)
        print "Wrote new: {0}".format(filepath)
