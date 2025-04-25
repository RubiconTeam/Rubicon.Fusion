using System.Collections.Generic;
using System.Linq;

namespace Rubicon;

/// <summary>
/// A general purpose utility class for Rubicon Engine
/// I'm not naming this CoolUtil bro - binpuki
/// :( - riven
/// </summary>
public static class RubiconUtility
{
	/// <summary>
	/// Creates a number based on the versions provided below
	/// </summary>
	/// <param name="major">The major build</param>
	/// <param name="minor">The minor build</param>
	/// <param name="patch"></param>
	/// <param name="build"></param>
	/// <returns>A number based on the four versions</returns>
	public static uint CreateVersion(byte major, byte minor, byte patch, byte build) => ((uint)major << 24) | ((uint)minor << 16) | ((uint)patch << 8) | build;
	
	/// <summary>
	/// Gets the first child that is of type specified.
	/// </summary>
	/// <typeparam name="T">The type</typeparam>
	/// <param name="node">The root node</param>
	/// <param name="includeInternal">Whether to recursively check for children inside</param>
	/// <returns>A child of type <see cref="T"/> if found, else null.</returns>
	public static T GetChildOfType<T>(this Node node, bool includeInternal = false) where T : class
	{
		return node.GetChildren(includeInternal).FirstOrDefault(x => x is T) as T;
	}

	/// <summary>
	/// Gets all children that is of type specified.
	/// </summary>
	/// <typeparam name="T">The type</typeparam>
	/// <param name="node">The root node</param>
	/// <param name="includeInternal">Whether to recursively check for children inside</param>
	/// <returns>An array of children of type <see cref="T"/> if found, else null.</returns>
	public static T[] GetChildrenOfType<T>(this Node node, bool includeInternal = false) where T : class
	{
		return node.GetChildren(includeInternal).Where(x => x is T).Cast<T>().ToArray();
	}

	public static string ToMemoryFormat(this ulong mem)
	{
		return mem switch
		{
			>= 0x40000000 => (float)Math.Round(mem / 1024f / 1024f / 1024f, 2) + " GB",
			>= 0x100000 => (float)Math.Round(mem / 1024f / 1024f, 2) + " MB",
			>= 0x400 => (float)Math.Round(mem / 1024f, 2) + " KB",
			_ => mem + " B"
		};
	}

	public static Vector2 GetGlobalPositionExcludeParallax(this Node2D node)
	{
		return ExcludeParallaxPositionLoop(node);
	}

	public static Vector2 GetGlobalPositionExcludeParallax(this Control control)
	{
		return ExcludeParallaxPositionLoop(control);
	}

	public static float GetGlobalRotation(this Control control)
	{
		string[] pathNames = RubiconEngine.Root.GetPathTo(control).ToString().Split('/');

		Node currentNode = RubiconEngine.Root;
		float rot = 0f;
		for (int i = 0; i < pathNames.Length; i++)
		{
			currentNode = currentNode.GetNode(pathNames[i]);
			
			if (currentNode is Control controlNode)
				rot += controlNode.Rotation;

			if (currentNode is Node2D node2D)
				rot += node2D.Rotation;
		}

		return rot;
	}

	public static Node GetClosest2DParent(this Node node)
	{
		Node currentNode = node.GetParent();
		while (currentNode != null)
		{
			if (currentNode is Node2D or Control)
				return currentNode;
			
			currentNode = currentNode.GetParent();
		}

		return null;
	}

	public static Node3D GetClosest3DParent(this Node node)
	{
		Node currentNode = node.GetParent();
		while (currentNode != null)
		{
			if (currentNode is Node3D node3D)
				return node3D;
			
			currentNode = currentNode.GetParent();
		}

		return null;
	}

	private static Vector2 ExcludeParallaxPositionLoop(Node node)
	{
		string[] pathNames = RubiconEngine.Root.GetPathTo(node).ToString().Split('/');
		
		Node lastValidNode = null;
		Node currentNode = RubiconEngine.Root;
		
		Vector2 position = Vector2.Zero;
		for (int i = 0; i < pathNames.Length; i++)
		{
			currentNode = currentNode.GetNode(pathNames[i]);
			if (currentNode is Parallax2D or ParallaxLayer or ParallaxBackground)
				continue;

			Node closestParent = lastValidNode;
			
			Vector2 posAdd = Vector2.Zero;
			if (currentNode is Control controlNode)
			{
				posAdd = controlNode.Position.Rotated(controlNode.Rotation);
				lastValidNode = controlNode;
			}

			if (currentNode is Node2D node2D)
			{
				posAdd = node2D.Position.Rotated(node2D.Rotation);
				lastValidNode = node2D;
			}
			
			if (closestParent is Node2D parent2d and not ParallaxLayer)
				posAdd *= parent2d.Scale;
			if (closestParent is Control controlParent)
				posAdd *= controlParent.Scale;
			
			position += posAdd;
		}

		return position;
	}
}
