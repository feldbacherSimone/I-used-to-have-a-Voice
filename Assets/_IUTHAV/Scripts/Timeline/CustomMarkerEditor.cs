namespace _IUTHAV.Scripts.Timeline {

using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

// Editor used by the Timeline window to customize the appearance of a NotesMarker

[CustomTimelineEditor(typeof(CustomMarker))]
public class CustomMarkerEditor : MarkerEditor {
    // Set a constant for the transparency of overlays
    const float k_OverlayAlpha = 0.5f;

    // Override this method to draw a vertical line over the Timeline window's contents.
    public override void DrawOverlay(IMarker marker, MarkerUIStates uiState, MarkerOverlayRegion region) {
        // Check if marker is not NotesMarker. Set notes as local variable.
        if (marker is not CustomMarker customMarker) {
            return; // If not, return without drawing an overlay
        }

        // If NotesMarker, check if Show Line Overlay property is true
        if (customMarker.ShowLineOverlay) {
            DrawLineOverlay(customMarker.GetColor(), region); // if Show Line Overlay is true, call function to draw vertical line
        }

        if (customMarker.ShowMarkerOverlay) {
            DrawMarkerOverlay(customMarker.GetColor(), region, MarkerUIStates.Selected);
        }
    }

    static void DrawLineOverlay(Color color, MarkerOverlayRegion region) {
        // Calculate a rectangle that uses the full timeline region's height and marker width
        Rect overlayLineRect = new Rect(region.markerRegion.x,
            region.timelineRegion.y,
            region.markerRegion.width / 4f,
            region.timelineRegion.height);

        // Set the color with an extra alpha value adjustment, then draw the rectangle
        Color overlayLineColor = new Color(color.r, color.g, color.b, color.a * k_OverlayAlpha);
        EditorGUI.DrawRect(overlayLineRect, overlayLineColor);
    }
    
    static void DrawMarkerOverlay(Color color, MarkerOverlayRegion region, MarkerUIStates state) {
        // By default, set the height to the markerRegion's height
        float markerHeight = region.markerRegion.height;

        // If marker is collapsed, set the height to 2/3 the markerRegion's height
        if (state.HasFlag(MarkerUIStates.Collapsed))
        {
            markerHeight = region.markerRegion.height / 1.5f;
        }

        // Calculate a rectangle that uses the marker region and variable markerHeight
        Rect overlayMarkerRect = new Rect(region.markerRegion.x,
            region.markerRegion.y,
            region.markerRegion.width,
            markerHeight);

        Color overlayMarkerColor = new Color(color.r, color.g, color.b, color.a * k_OverlayAlpha);
        EditorGUI.DrawRect(overlayMarkerRect, overlayMarkerColor);
    }

}

}