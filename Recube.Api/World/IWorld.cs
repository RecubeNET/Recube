using System;
using System.Threading.Tasks;
using Recube.Api.Block;

namespace Recube.Api.World
{
    /// <summary>
    /// This interface represents a world
    /// </summary>
    public interface IWorld
    {
        /// <summary>
        /// The name of this world
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Sets a block at the given coordinates
        /// </summary>
        /// <param name="location">The location</param>
        /// <param name="block">The block</param>
        /// <returns>True if the block's network id has been found, false otherwise.</returns>
        public bool SetBlock(Location location, BaseBlock block);

        /// <summary>
        /// Gets the block at the given coordinates
        /// </summary>
        /// <param name="location">The location</param>
        /// <returns>The block or null if the block instance could not be created</returns>
        public BaseBlock? GetBlock(Location location);

        /// <summary>
        /// Gets the block at the given coordinates
        /// </summary>
        /// <param name="location">The location</param>
        /// <typeparam name="T">The expected block</typeparam>
        /// <returns>The block or null if the block instance could not be created or the block type differs from T</returns>
        public T? GetBlock<T>(Location location) where T : BaseBlock;

        /// <summary>
        /// Sets the block type at the given coordinates directly.
        /// For the most people/usages sticking with <see cref="SetBlock"/> is easier and more future-safe, because network ids can change
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        /// <param name="type">The network id of your desired block</param>
        public void SetType(int x, int y, int z, int type);

        /// <summary>
        /// Gets the block type at the given coordinates.
        /// For the most people/usages sticking with <see cref="GetBlock"/> is easier and more future-safe, because network ids can change
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        /// <returns>The network id of the type</returns>
        public int GetType(int x, int y, int z);

        /// <summary>
        /// Runs the given action in the world thread's thread
        /// </summary>
        /// <param name="action">The action</param>
        /// <returns>A task which completes when the action has been executed</returns>
        public Task Run(Func<Task> action);
    }
}