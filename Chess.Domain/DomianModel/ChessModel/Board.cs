﻿using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.Events;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects;
using Chess.Domain.Extensions;
using Microservice.Framework.Domain.Aggregates;
using Microservice.Framework.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel
{
    public class Board : AggregateRoot<Board, BoardId>
    {
        private IList<Block> _blocks;

        #region Constructors

        public Board()
            : base(null)
        {

        }

        public Board(BoardId boardId)
            : base(boardId)
        {

        }

        #endregion

        #region Properties

        private Action<object, string> LazyLoader { get; set; }

        public IList<Block> Blocks 
        {
            get => LazyLoader.Load(this, ref _blocks);
            set => _blocks = value;
        }

        #endregion

        #region Methods

        public void CreateBoard(IReadOnlyList<Block> blocks)
        {
            Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this);
            Blocks = blocks.ToList();
            Emit(new CreatedBoardEvent());
        }

        public void MovePiece(Move move)
        {
            Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

            move.Specification(new ReadOnlyCollection<Block>(Blocks))
                .ThrowDomainErrorIfNotSatisfied(move);

            //If we got at this point, the move was valid
            Move(move);

            Emit(new MovedPieceEvent(move));
        }

        public void ResetBoard()
        {
            Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
            Emit(new ResetBoardEvent());
        }

        #endregion

        #region Private Methods

        private void Move(Move move)
        {
            ////Where we are moving from block
            //var occupiedBlock = Blocks.First(b =>
            //                     b.ChessPiece?.Id == move.PieceId);

            ////Where we are moving from index
            //var occupiedBlockIdx = Blocks
            //    .IndexOf(occupiedBlock);

            ////Where we are moving to index
            //var newBlockIdx = Blocks
            //    .IndexOf(Blocks.First(b => 
            //                    b.XCoordinate == move.NewXCoordinate 
            //                        && b.YCoordinate == move.NewYCoordinate));

            ////Keep a reference of the piece we want to move
            //var pieceToMove = GetPiece(move.PieceId);

            ////Now move the piece to the new block
            //Blocks[newBlockIdx].ChessPiece = new ChessPiece
            //{
            //    Id = pieceToMove.Id,
            //    PieceColor = pieceToMove.PieceColor,
            //    PieceName = pieceToMove.PieceName,
            //    XCoordinate = move.NewXCoordinate,
            //    YCoordinate = move.NewYCoordinate
            //};

            ////Clean the last block that was occupied
            //Blocks[occupiedBlockIdx].ChessPiece = null;
        }

        #endregion
    }
}
