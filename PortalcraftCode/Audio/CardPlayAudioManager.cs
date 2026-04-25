using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace sts2_char_portalcraft.PortalcraftCode.Audio;

public static class CardPlayAudioManager
{
    private static readonly Dictionary<string, AudioStream> _cache = new();
    private static AudioStreamPlayer[] _players;
    private static int _nextPlayer;
    private static bool _initialized;
    private static float _volumeDb = -6f;

    private static readonly HashSet<string> _addToHandTypes = new()
    {
        "WhitePsalmNewRevelation",
        "BlackPsalmNewRevelation",
    };

    public static bool IsAddToHandType(string typeName) => _addToHandTypes.Contains(typeName);

    public static void SetVolume(float volumeDb)
    {
        _volumeDb = volumeDb;
        if (_players == null) return;
        foreach (var player in _players)
            player.VolumeDb = volumeDb;
    }

    public static void Initialize()
    {
        if (_initialized) return;

        _players = new AudioStreamPlayer[3];
        var root = ((SceneTree)Engine.GetMainLoop()).Root;

        for (int i = 0; i < _players.Length; i++)
        {
            _players[i] = new AudioStreamPlayer();
            _players[i].VolumeDb = _volumeDb;
            root.CallDeferred(Node.MethodName.AddChild, _players[i]);
        }

        _initialized = true;
        MainFile.Logger.Info("CardPlayAudioManager initialized");
    }

    public static void PlayForCard(string typeName)
    {
        if (!_initialized || NonInteractiveMode.IsActive) return;
        if (_volumeDb <= -20f) return;

        var stream = GetOrLoadStream(typeName);
        if (stream == null)
        {
            if (typeName.EndsWith("SuperEvolved"))
                stream = GetOrLoadStream(typeName[..^"SuperEvolved".Length]);
            else if (typeName.EndsWith("Evolved"))
                stream = GetOrLoadStream(typeName[..^"Evolved".Length]);
        }
        if (stream == null) return;

        var player = _players[_nextPlayer];
        _nextPlayer = (_nextPlayer + 1) % _players.Length;
        player.Stream = stream;
        player.Play();
    }
    
    public static void PlayForEvolve(string typeName) => PlayForCard(typeName + "Evolve");
    public static void PlayForSuperEvolve(string typeName) => PlayForCard(typeName + "SuperEvolve");

    private static readonly Queue<AudioStream> _effectQueue = new();
    private static bool _effectTimerActive;

    public static void PlayForEffect(string typeName)
    {
        if (!_initialized || NonInteractiveMode.IsActive) return;
        if (_volumeDb <= -20f) return;

        var stream = GetOrLoadStream(typeName + "Effect");
        if (stream == null) return;

        _effectQueue.Enqueue(stream);
        if (!_effectTimerActive) ScheduleNextEffect();
    }

    private static void ScheduleNextEffect()
    {
        _effectTimerActive = true;
        var tree = (SceneTree)Engine.GetMainLoop();
        var timer = tree.CreateTimer(0.5);
        timer.Timeout += OnEffectTimerFired;
    }

    private static void OnEffectTimerFired()
    {
        if (_effectQueue.Count == 0)
        {
            _effectTimerActive = false;
            return;
        }

        var stream = _effectQueue.Dequeue();
        if (_volumeDb > -20f)
        {
            var player = _players[_nextPlayer];
            _nextPlayer = (_nextPlayer + 1) % _players.Length;
            player.Stream = stream;
            player.Play();
        }

        if (_effectQueue.Count > 0)
            ScheduleNextEffect();
        else
            _effectTimerActive = false;
    }

    private static AudioStream GetOrLoadStream(string typeName)
    {
        if (_cache.TryGetValue(typeName, out var cached))
            return cached;

        var path = $"res://{MainFile.ModId}/audio/{typeName}.ogg";
        if (!ResourceLoader.Exists(path))
        {
            _cache[typeName] = null;
            return null;
        }

        var stream = ResourceLoader.Load<AudioStream>(path);
        _cache[typeName] = stream;
        return stream;
    }
}
