Imports System.IO
Imports System.IO.File
Imports System.Net
Imports System.Security.Cryptography.X509Certificates
Imports System.Security
Imports System.Net.Security
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json

Public Class Form1
    ' Allow self-signed certificates
    Public Function AcceptAllCertifications(ByVal sender As Object, ByVal certification As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, ByVal sslPolicyErrors As System.Net.Security.SslPolicyErrors) As Boolean
        Return True
    End Function
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Enable the controls
        PictureBox1.Enabled = True
        RichTextBox1.Enabled = True
        TextBox3.Enabled = True
        TextBox4.Enabled = True
        TextBox5.Enabled = True
        Label3.Enabled = True
        Label4.Enabled = True
        Label5.Enabled = True
        Label6.Enabled = True
        Label7.Enabled = True

        Try
            ' Makes sure both boxes are filled
            If TextBox1.Text = "" Or ComboBox1.Text = "" Then

                MsgBox("Please enter a title ID and select a region!")

            Else
                ' Send cert-signed HTTPS GET requests to shogun
                ' Force no certificate validation
                ServicePointManager.ServerCertificateValidationCallback = AddressOf AcceptAllCertifications
                ' Define the "TID" variable as the text in TextBox1 (The title ID field)
                Dim TID As String = TextBox1.Text
                ' Define the "REG" variable as the region chosen from the combo box
                Dim REG As String = ComboBox1.Text
                ' Generate the ID pair URL based on inputs
                Dim GetIDPair As String = "https://bugyo.hac.lp1.eshop.nintendo.net/shogun/v1/contents/ids?shop_id=4&lang=en&country=" + REG + "&type=title&title_ids=" + TID
                ' Use the cert "ShopN.p12" with the password "shop"
                Dim Cert As New X509Certificate2("ShopN.p12", "shop")
                ' Create the web request
                Dim request As HttpWebRequest = WebRequest.Create(GetIDPair)
                ' Add the cert to the request
                request.ClientCertificates.Add(Cert)
                ' Get the response
                Dim response As HttpWebResponse = request.GetResponse
                ' Parse the response as a text stream
                Dim Rd As StreamReader = New StreamReader(response.GetResponseStream)
                ' Read the entire response
                Dim IDPair As String = Rd.ReadToEnd
                ' Return the NSU ID only and write it to Label3
                Label3.Text = IDPair.Substring(19, 14)
                ' Define the NSU ID as Label3's text
                Dim NSUID As String = Label3.Text
                ' Generate the JSON URL based on inputs
                Dim GetInfo As String = "https://bugyo.hac.lp1.eshop.nintendo.net/shogun/v1/titles/" + NSUID + "?shop_id=4&lang=en&country=" + REG
                ' Create the web request
                Dim request2 As HttpWebRequest = WebRequest.Create(GetInfo)
                ' Add the cert to the request
                request2.ClientCertificates.Add(Cert)
                ' Get the response
                Dim response2 As HttpWebResponse = request2.GetResponse
                ' Parse the response as a text stream
                Dim Rd2 As StreamReader = New StreamReader(response2.GetResponseStream)
                ' Read the entire response
                Dim Info As String = Rd2.ReadToEnd
                ' Parse the returned JSON
                Dim jResults As JObject = JObject.Parse(Info)
                ' Check to see if the title is public
                If jResults("public_status").ToString() = "public" Then
                    ' Gets the game name from the "formal_name" object
                    TextBox3.Text = jResults("formal_name").ToString()
                    ' Gets the description from the "description" object
                    RichTextBox1.Text = jResults("description").ToString()
                    ' Gets the release date from the "release_date_on_eshop" object
                    TextBox4.Text = jResults("release_date_on_eshop").ToString()
                    ' Gets the publisher name from the "name" field in the "publisher" object
                    TextBox5.Text = jResults("publisher")("name").ToString()
                    ' Gets the image ID from the "hero_banner_url" object
                    Dim IMGID As String = jResults("hero_banner_url").ToString()
                    ' Generate the full image URL.
                    Dim GetImg As String = "https://bugyo.hac.lp1.eshop.nintendo.net/" + IMGID + "?w=640"
                    ' Opens a request to the URL
                    Dim request3 As HttpWebRequest = WebRequest.Create(GetImg)
                    ' Add certificate to request
                    request3.ClientCertificates.Add(Cert)
                    ' Get the response
                    Dim response3 As HttpWebResponse = request3.GetResponse
                    ' Parse the response, reading as binary data
                    Dim Rd3 As BinaryReader = New BinaryReader(response3.GetResponseStream)
                    ' Save the image data as temp.jpg
                    WriteAllBytes("temp.jpg", Rd3.ReadBytes(8008135))
                    ' Display the saved image the picture box
                    PictureBox1.ImageLocation = "temp.jpg"
                    Try
                        ' If colours are defined, set form and controls to them
                        ' Read colours from the "dominant_colors" object
                        Dim GetBC = jResults("dominant_colors")(0).ToString()
                        Dim NewBC = ColorTranslator.FromHtml("#" + GetBC)
                        Dim GetFC = jResults("dominant_colors")(1).ToString()
                        Dim NewFC = ColorTranslator.FromHtml("#" + GetFC)
                        Dim GetSC = jResults("dominant_colors")(2).ToString()
                        Dim NewSC = ColorTranslator.FromHtml("#" + GetSC)
                        Me.BackColor = NewBC
                        Label1.ForeColor = NewFC
                        Label2.ForeColor = NewFC
                        Label3.ForeColor = NewFC
                        Label4.ForeColor = NewFC
                        Label5.ForeColor = NewFC
                        Label6.ForeColor = NewFC
                        Label7.ForeColor = NewFC
                        Button1.ForeColor = NewFC
                        TextBox1.ForeColor = NewFC
                        TextBox3.ForeColor = NewFC
                        TextBox4.ForeColor = NewFC
                        TextBox5.ForeColor = NewFC
                        RichTextBox1.ForeColor = NewFC
                        ComboBox1.ForeColor = NewFC
                        TextBox1.BackColor = NewBC
                        TextBox3.BackColor = NewBC
                        TextBox4.BackColor = NewBC
                        TextBox5.BackColor = NewBC
                        RichTextBox1.BackColor = NewBC
                        Button1.BackColor = NewBC
                        ComboBox1.BackColor = NewBC
                    Catch NoColor As System.NullReferenceException
                        ' If there's a title with no colour, reset all to default
                        Me.BackColor = DefaultBackColor
                        Label1.ForeColor = DefaultForeColor
                        Label2.ForeColor = DefaultForeColor
                        Label3.ForeColor = DefaultForeColor
                        Label4.ForeColor = DefaultForeColor
                        Label5.ForeColor = DefaultForeColor
                        Label6.ForeColor = DefaultForeColor
                        Label7.ForeColor = DefaultForeColor
                        Button1.ForeColor = DefaultForeColor
                        TextBox1.ForeColor = DefaultForeColor
                        TextBox3.ForeColor = DefaultForeColor
                        TextBox4.ForeColor = DefaultForeColor
                        TextBox5.ForeColor = DefaultForeColor
                        RichTextBox1.ForeColor = DefaultForeColor
                        ComboBox1.ForeColor = DefaultForeColor
                        TextBox1.BackColor = DefaultBackColor
                        TextBox3.BackColor = DefaultBackColor
                        TextBox4.BackColor = DefaultBackColor
                        TextBox5.BackColor = DefaultBackColor
                        RichTextBox1.BackColor = DefaultBackColor
                        Button1.BackColor = DefaultBackColor
                        ComboBox1.BackColor = DefaultBackColor
                    End Try

                Else
                    ' If it returns a non-public or cartridge-only title (e.g. Labo, Skylanders, etc.)
                    MsgBox("This title exists, but isn't listed on the eShop.")
                End If
            End If
        Catch ex As System.ArgumentOutOfRangeException
            ' If it detects an ID ending in "800", assume it's an update and replace it with "000"
            If TextBox1.Text.Substring(13, 3) = "800" Then
                MsgBox("This is an update ID!" + vbNewLine + "Because I'm nice, I'll give you the base title ID." + vbNewLine + "BUT DON'T DO THIS AGAIN!!!")
                TextBox1.Text = TextBox1.Text.Remove(13, 3)
                TextBox1.AppendText("000")
            Else
                MsgBox("Invalid input! Please check the title ID and region.")
            End If
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Checks to make sure the cert exists
        If File.Exists("ShopN.p12") Then
        Else
            MsgBox("Please put the eShop certificate (ShopN.p12) in this folder.")
            Close()
        End If
    End Sub
    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        ' Delete the temporary image when the program is closed
        File.Delete("temp.jpg")
    End Sub
End Class
