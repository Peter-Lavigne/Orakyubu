using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModelEventPayload
{
  public IEnumerable<Level> levels;
  public IEnumerable<Box3D> box3ds;
  public IEnumerable<Plane2D> planes;
  public IEnumerable<Object2D> player2ds;
  public IEnumerable<Object2D> box2ds;
  public IEnumerable<Object2D> targets;
  public IEnumerable<Object2D> walls;

  // set any field
  public ModelEventPayload(
    IEnumerable<Level> _levels = null,
    IEnumerable<Box3D> _box3ds = null,
    IEnumerable<Plane2D> _planes = null,
    IEnumerable<Object2D> _player2ds = null,
    IEnumerable<Object2D> _box2ds = null,
    IEnumerable<Object2D> _targets = null,
    IEnumerable<Object2D> _walls = null
  ) {
    levels    = _levels    != null ? _levels    : new List<Level>();
    box3ds    = _box3ds    != null ? _box3ds    : new List<Box3D>();
    planes    = _planes    != null ? _planes    : new List<Plane2D>();
    player2ds = _player2ds != null ? _player2ds : new List<Object2D>();
    box2ds    = _box2ds    != null ? _box2ds    : new List<Object2D>();
    targets   = _targets   != null ? _targets   : new List<Object2D>();
    walls     = _walls     != null ? _walls     : new List<Object2D>();
  }

  // update entire levels and their children
  public ModelEventPayload(
    IEnumerable<Level> _levels
  ) {
    levels = _levels;
    box3ds = levels.SelectMany(level => level.box3ds);
    planes = box3ds.SelectMany(box => box.faces);
    player2ds = planes.SelectMany(plane => plane.players);
    box2ds = planes.SelectMany(plane => plane.boxes);
    targets = planes.SelectMany(plane => plane.goals);
    walls = planes.SelectMany(plane => plane.walls);
  }

  public static ModelEventPayload FromCubes(IEnumerable<Box3D> cubes) {
    IEnumerable<Plane2D> planes = cubes.SelectMany(cube => cube.faces);
    return new ModelEventPayload(
      _box3ds: cubes,
      _planes: planes,
      _player2ds: planes.SelectMany(plane => plane.players),
      _box2ds: planes.SelectMany(plane => plane.boxes),
      _targets: planes.SelectMany(plane => plane.goals),
      _walls: planes.SelectMany(plane => plane.walls)
    );
  }
}
