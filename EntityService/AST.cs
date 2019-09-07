using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CalculatorApp.EntityService
{
    public class TreeNode<T>
    {
        public TreeNode<T> Left { get; set; }
        public TreeNode<T> Right { get; set; }
        public T Data;

        public TreeNode(T data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// A simple abstarct syntax tree for evaluation
    /// </summary>
    public class AST
    {
        public TreeNode<Token> Root { get; set; }

        public Action<Token> EvaluationCallBack { get; set; }

        public AST(IEnumerable<Token> tokens)
        {
            Root = BuildTree(tokens);
        }

        /// <summary>
        /// Build the tree from identified tokens
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private TreeNode<Token> BuildTree(IEnumerable<Token> tokens)
        {
            var ts = tokens.ToArray();

            if (ts.Length == 1)
            {
                return new TreeNode<Token>(ts[0]);
            }
            else if (ts.Length == 2)
                throw new InvalidSyntaxException(ts[1]);
            else if (ts.Length == 0)
                throw new ArgumentException(nameof(tokens));

            var parent = Assemble(new TreeNode<Token>(ts[0]), new TreeNode<Token>(ts[1]), new TreeNode<Token>(ts[2]));

            for (int i = 3; i < ts.Length; i += 2)
            {
                if (ts[i].Type == TokenType.PlusOperator || ts[i].Type == TokenType.MinusOperator)
                {
                    parent = Assemble(parent, new TreeNode<Token>(ts[i]), new TreeNode<Token>(ts[i + 1]));
                }
                else if (ts[i].Type == TokenType.MultiplyOperator || ts[i].Type == TokenType.DivideOperator)
                {
                    parent.Right = Assemble(parent.Right, new TreeNode<Token>(ts[i]), new TreeNode<Token>(ts[i + 1]));
                }
                else
                {
                    throw new InvalidSyntaxException(ts[i]);
                }
            }

            return parent;
        }

        /// <summary>
        /// Evaluate AST to calculate result
        /// </summary>
        /// <returns></returns>
        public int? Evaluate()
        {
            return Evaluate(Root);
        }

        private int? Evaluate(TreeNode<Token> node, TreeNode<Token> parent = null)
        {
            if (node.Data.Type == TokenType.Number)
            {
                EvaluationCallBack?.Invoke(node.Data);
                return node.Data.Value;
            }
            else if (node.Data.Type == TokenType.IgnoredNumber)
            {
                if (parent == null)
                    node.Data.Value = 0;
                else if (parent.Data.Type == TokenType.PlusOperator || parent.Data.Type == TokenType.MinusOperator)
                    node.Data.Value = 0;
                else if (parent.Data.Type == TokenType.MultiplyOperator || parent.Data.Type == TokenType.DivideOperator)
                    node.Data.Value = 1;
                else
                    throw new InvalidSyntaxException(node.Data);
                EvaluationCallBack?.Invoke(node.Data);
                return node.Data.Value;
            }
               
            var leftValue = Evaluate(node.Left, node);
            EvaluationCallBack?.Invoke(node.Data);
            var rightValue = Evaluate(node.Right, node);

            switch (node.Data.Type)
            {
                case TokenType.PlusOperator:
                    return leftValue + rightValue;
                case TokenType.MinusOperator:
                    return leftValue - rightValue;
                case TokenType.MultiplyOperator:
                    return leftValue * rightValue;
                case TokenType.DivideOperator:
                    return leftValue / rightValue;
                default:
                    throw new InvalidSyntaxException(node.Data);
            }
        }


        private TreeNode<Token> Assemble(TreeNode<Token> leftChild, TreeNode<Token> parent, TreeNode<Token> rightChild)
        {
            if(parent.Data.Type != TokenType.PlusOperator && parent.Data.Type != TokenType.MinusOperator 
                && parent.Data.Type != TokenType.MultiplyOperator && parent.Data.Type != TokenType.DivideOperator)
                throw new InvalidSyntaxException(parent.Data);

            parent.Left = leftChild;
            parent.Right = rightChild;
            return parent;
        }
    }
}
