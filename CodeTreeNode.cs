using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kursaDarbs
{
    public class CodeTreeNode : IComparable<CodeTreeNode> {

        public Char? content;
        public int weight;
        public CodeTreeNode left;
        public CodeTreeNode right;

        public CodeTreeNode(Char? content, int weight)
        {
            this.content = content;
            this.weight = weight;
        }

        public CodeTreeNode(Char? content, int weight, CodeTreeNode left, CodeTreeNode right)
        {
            this.content = content;
            this.weight = weight;
            this.left = left;
            this.right = right;
        }

    
        public int CompareTo(CodeTreeNode o)
        {
            return o.weight - weight;
        }

    // извлечение кода для символа
        public String getCodeForCharacter(Char? ch, String parentPath)
        {
            if (content == ch)
            {
                return parentPath;
            }
            else
            {
                if (left != null)
                {
                    String path = left.getCodeForCharacter(ch, parentPath + 0);
                    if (path != null)
                    {
                        return path;
                    }
                }
                if (right != null)
                {
                    String path = right.getCodeForCharacter(ch, parentPath + 1);
                    if (path != null)
                    {
                        return path;
                    }
                }
            }
            return null;
        }

    }
}
