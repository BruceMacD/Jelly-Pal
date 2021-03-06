﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Beat : MonoBehaviour
{

    // Which beat in the song does this correspond to
    public int songBeatPosition;
    // The beat this was initialized on
    public int beatInitAdvance;

    // A buffer time to wait before hiding a missed beat
    private float BUFFER_TIME = 0.1f;
    // Score distance
    private decimal MISS = 1.5M, CLOSE = 0.4M, GOOD = 0.2M;

    private Vector2 startPoint;
    private Vector2 endPoint;
    private float step;
    // Score to be updated
    private Text score;
    private Text beatFeedback;
    private ParticleSystem fireworks;

    public void ClickedBeat()
    {
        decimal distance = (decimal)Mathf.Abs(transform.position.y - endPoint.y);
        UpdateFeedback(distance);
        UpdateScore(distance);
        // Remove the beat
        Destroy(gameObject);
    }
    
    void Start()
    {
        // find the scoreboard and feedback text on the screen
        score = GameObject.Find("Score Amount Text").GetComponent<Text>();
        beatFeedback = GameObject.Find("Feedback Text").GetComponent<Text>();
        transform.SetParent(GameObject.Find("Canvas").transform, false);
        endPoint = GameObject.Find("Beat Target").transform.position;
        fireworks = GameObject.Find("Particle Systems").GetComponent<ParticleSystem>();

        // set random position
        float randomXStart = Random.Range(-2.5f, 2.5f);
        startPoint = transform.position;
        startPoint.x = randomXStart;
        endPoint.x = randomXStart;
        transform.position = startPoint;
    }
    
    void Update()
    {
        // Remove the beat if it has been missed
        if (SongPlayer.beatTime > (songBeatPosition + BUFFER_TIME))
        {
            beatFeedback.text = "MISS";
            SongPlayer.missed += 1;
            Destroy(gameObject);
        }

        // The step function linearly approaches an asymptote at 1, from https://www.gamasutra.com/blogs/YuChao/20170316/293814/Music_Syncing_in_Rhythm_Games.php
        step = (beatInitAdvance - (songBeatPosition - SongPlayer.beatTime)) / beatInitAdvance;

        transform.position = Vector2.Lerp(
            startPoint,
            endPoint,
            step
        );
    }

    // Update the score based on the distance from the beat to the goal
    private void UpdateScore(decimal distance)
    {
        // This will always convert, no need to catch errors
        int currentScore = System.Int32.Parse(score.text);
        int noteScore = 0;
        // turn off fireworks unless the user gets perfect
        fireworks.Play(false);
        if (distance > CLOSE)
        {
            noteScore = 5;
        } else if (distance > GOOD) {
            noteScore = 20;
        } else {
            // Hit beat
            noteScore = 50;
            fireworks.Play(true);
            // forgive a beat miss
            if (SongPlayer.missed > 0)
            {
                SongPlayer.missed -= 1;
            }
        }
        int newScore = currentScore + noteScore;
        // Update the display
        score.text = newScore.ToString();
     }

    // Update the feedback based on the distance from the beat to the goal
    private void UpdateFeedback(decimal distance)
    {
        if (distance > MISS)
        {
            // no score added
            beatFeedback.text = "MISS";
            SongPlayer.missed += 1;
        }
        else if (distance > CLOSE)
        {
            beatFeedback.text = "CLOSE";
        }
        else if (distance > GOOD)
        {
            beatFeedback.text = "GOOD";
        }
        else
        {
            // Hit beat
            beatFeedback.text = "PERFECT";
        }
    }
}
