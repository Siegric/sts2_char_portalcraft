using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Audio;

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

        var stream = GetOrLoadStream(typeName);
        if (stream == null) return;

        var player = _players[_nextPlayer];
        _nextPlayer = (_nextPlayer + 1) % _players.Length;
        player.Stream = stream;
        player.Play();
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
