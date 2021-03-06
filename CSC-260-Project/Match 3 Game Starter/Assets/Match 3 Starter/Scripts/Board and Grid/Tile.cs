/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tile : MonoBehaviour
{
	public static Tile instance;
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected = null;

	private SpriteRenderer render;
	private bool isSelected = false;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
	public bool matchFound = false;
	
	



	void Awake()
	{
		render = GetComponent<SpriteRenderer>();
	}

	private void Select()
	{
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect()
	{
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}

	void OnMouseDown()
	{
		if (render.sprite == null || BoardManager.instance.IsShifting)
		{
			return;
		}

		if (isSelected)
		{
			Deselect();
		}
		else
		{
			if (previousSelected == null)
			{
				Select();
			}
			else
			{
				if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
				{
					CountdownTimer.instance.matchCleared = false;
					SwapSprite(previousSelected.render);
					previousSelected.ClearAllMatches();
					previousSelected.Deselect();
					ClearAllMatches();
					if (CountdownTimer.instance.matchCleared == false)
					{
						CountdownTimer.instance.combo = 0;
						StartCoroutine(CountdownTimer.instance.ShowCombo());


					}

				}
				else
				{
					previousSelected.GetComponent<Tile>().Deselect();
					Select();
				}
			}
		}
	}

	public void SwapSprite(SpriteRenderer render2)
	{
		if (render.sprite == render2.sprite)
		{
			return;
		}

		Sprite tempSprite = render2.sprite;
		render2.sprite = render.sprite;
		render.sprite = tempSprite;
		SFXManager.instance.PlaySFX(Clip.Swap);
		GUIManager.instance.MoveCounter--;      // decrements the move counter after every swap, regardless if a match or not - JP
	}

	private GameObject GetAdjacent(Vector2 castDir)
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
		if (hit.collider != null)
		{
			return hit.collider.gameObject;
		}
		return null;
	}

	private List<GameObject> GetAllAdjacentTiles() // Only allows adjacent tiles to be swapped - JP
	{
		List<GameObject> adjacentTiles = new List<GameObject>();
		for (int i = 0; i < adjacentDirections.Length; i++)
		{
			adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
		}
		return adjacentTiles;
	}

	private List<GameObject> FindMatch(Vector2 castDir)
	{
		List<GameObject> matchingTiles = new List<GameObject>();
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
		while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
		{
			matchingTiles.Add(hit.collider.gameObject);
			hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
		}
		return matchingTiles;
	}

	private void ClearMatch(Vector2[] paths)
	{
		List<GameObject> matchingTiles = new List<GameObject>();
		
		for (int i = 0; i < paths.Length; i++)
		{
			matchingTiles.AddRange(FindMatch(paths[i]));
		}
		// Give a flat 100 points for a match 3
		if (matchingTiles.Count >= 2)
		{
		
			
			for (int i = 0; i < matchingTiles.Count; i++)
			{
				matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;

			}
			matchFound = true;

			GUIManager.instance.Score += (150 * (matchingTiles.Count - 1));
			if (CountdownTimer.instance.combo > 1)
            {
				GUIManager.instance.Score += 150 * CountdownTimer.instance.combo;
			
				StartCoroutine(CountdownTimer.instance.ShowCombo());
			}
			CountdownTimer.instance.timeRemaining += 1;
			//timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
			//CountdownTimer.instance.addedTime.text = string.Format("+2");
			StartCoroutine(CountdownTimer.instance.ShowaddedTime("+1", .4f));
			CountdownTimer.instance.combo++;
			CountdownTimer.instance.matchCleared = true;
			

		}
		




	}

	public void ClearAllMatches()
	{
		List<GameObject> matchingTiles = new List<GameObject>();
		if (render.sprite == null)
		{
			return;
		}

		ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
		ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
		if (matchFound)
		{
			render.sprite = null;
			matchFound = false;
			StopCoroutine(BoardManager.instance.FindNullTiles());
			StartCoroutine(BoardManager.instance.FindNullTiles());
			SFXManager.instance.PlaySFX(Clip.Clear);
			//GUIManager.instance.MoveCounter--;		// was causing the moves counter to decrement by 2 with a match - JP
			
			
		}

		
	}

	
}